using AvistaRD.Application.Contracts;
using AvistaRD.Domain.Users;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using AvistaRD.Infrastructure.Identity;
using AvistaRD.Infrastructure.Options;
using AvistaRD.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AvistaRD.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Scoped)]
public class DatabaseInitializerService : IDatabaseInitializerService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<AdminAccountOptions> _adminAccountOptions;
    private readonly ILogger<DatabaseInitializerService> _logger;

    public DatabaseInitializerService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IOptions<AdminAccountOptions> adminAccountOptions,
        ILogger<DatabaseInitializerService> logger)
    {
        _context = context;
        _userManager = userManager;
        _adminAccountOptions = adminAccountOptions;
        _logger = logger;
    }
    
    public async Task CanConnectAsync(CancellationToken cancellationToken = default)
    {
        bool isConnected = await _context.Database.CanConnectAsync(cancellationToken);

        if (!isConnected)
        {
            _logger.LogWarning("Cant connect to the database, database not available");

            throw new InvalidOperationException();
        }

        _logger.LogInformation("Successfully connect to the database!!!.");
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Successfully apply the migrations!!.");

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An error happen trying to apply migrations to the database {providerName} with the error message: {message}",
                _context.Database.ProviderName,
                ex.Message);

            throw;
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        AdminAccountOptions adminAccount = _adminAccountOptions.Value;

        if (string.IsNullOrWhiteSpace(adminAccount.Email) || string.IsNullOrWhiteSpace(adminAccount.Password))
        {
            IList<ApplicationUser> administrators = await _userManager.GetUsersInRoleAsync(Roles.Administrator);

            if (administrators.Count == 0)
            {
                _logger.LogWarning(
                    "There is no administrator account. Set ADMIN_EMAIL and ADMIN_PASSWORD to create one at startup.");
            }

            return;
        }

        ApplicationUser? administrator = await _userManager.FindByEmailAsync(adminAccount.Email);

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                UserName = adminAccount.Email,
                Email = adminAccount.Email,
                EmailConfirmed = true,
                FullName = "Administrador",
                Status = UserStatus.Active
            };

            IdentityResult createResult = await _userManager.CreateAsync(administrator, adminAccount.Password);

            if (!createResult.Succeeded)
            {
                _logger.LogError(
                    "The administrator account {Email} couldn't be created: {Errors}",
                    adminAccount.Email,
                    string.Join(" ", createResult.Errors.Select(error => error.Description)));

                return;
            }

            _logger.LogInformation("Created the administrator account {Email}.", adminAccount.Email);
        }

        if (!await _userManager.IsInRoleAsync(administrator, Roles.Administrator))
        {
            IdentityResult roleResult = await _userManager.AddToRoleAsync(administrator, Roles.Administrator);

            if (!roleResult.Succeeded)
            {
                _logger.LogError(
                    "The role {Role} couldn't be added to {Email}: {Errors}",
                    Roles.Administrator,
                    adminAccount.Email,
                    string.Join(" ", roleResult.Errors.Select(error => error.Description)));
            }
        }
    }
}