using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using AvistaRD.Infrastructure.Options.Abstractions;
using Microsoft.Extensions.Configuration;

namespace AvistaRD.Infrastructure.Options;

// First administrator account, created at startup when it doesn't exist
public sealed class AdminAccountOptions : IOptionsModel
{
    public string SectionName => "AdminAccount";

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

// No validator on purpose: without ADMIN_EMAIL and ADMIN_PASSWORD the seed is skipped instead of failing at startup
[Inject(ServiceKind.Options)]
public class AdminAccountOptionsConfigurator : BaseOptions<AdminAccountOptions>
{
    public AdminAccountOptionsConfigurator(IConfiguration configuration) : base(configuration) { }

    public override void Configure(AdminAccountOptions options)
    {
        base.Configure(options);

        options.Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? options.Email;

        options.Password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? options.Password;
    }
}
