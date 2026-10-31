using AvistaRD.Infrastructure.Extensions;
using AvistaRD.Infrastructure.Options;
using AvistaRD.Infrastructure.Persistence;
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
            })
                .UseSnakeCaseNamingConvention();
            
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
        
        services
            .AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        return services;
    }
}