using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Infrastructure.Extensions;
using AvistaRD.Infrastructure.Identity;
using AvistaRD.Infrastructure.Options;
using AvistaRD.Infrastructure.Persistence;
using AvistaRD.Infrastructure.Persistence.Interceptors;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AvistaRD.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IHostEnvironment environment)
    {
        services.RegisterServicesFromAssembly(typeof(InfrastructureServiceRegistration).Assembly);
        
        services.AddValidatorsFromAssembly(typeof(InfrastructureServiceRegistration).Assembly);
        
        services.AddHttpContextAccessor();
        
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            DatabaseOptions databaseOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            options.UseSqlServer(databaseOptions.ConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(databaseOptions.CommandTimeout); 
            }) .AddInterceptors(
                provider.GetRequiredService<AuditableEntityInterceptor>())
                .UseSnakeCaseNamingConvention();
            
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
        
        // Identity and its bearer tokens, the API's authentication scheme. The endpoints are our own
        // (AuthController) instead of MapIdentityApi, to choose the role and the account status at registration.
        services
            .AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                // The email is also the user name, so it must be unique
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        
        return services;
    }
}