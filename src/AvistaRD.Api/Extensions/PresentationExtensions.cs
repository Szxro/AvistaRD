using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AvistaRD.Api.ExceptionHandlers;
using AvistaRD.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AvistaRD.Api.Extensions;

public static class PresentationExtensions
{
     public static IServiceCollection AddPresentationLayer(this IServiceCollection services, IHostEnvironment env)
    {
        // Authentication (Identity bearer tokens) is registered by AddIdentityApiEndpoints in the Infrastructure layer
        services.AddAuthorization();

        // Core Services: enums travel as text ("Pending" instead of 0)
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        // BaseController returns IResult (Results.Ok, Results.Problem), which serializes with these options instead
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();

        //Swagger
        services.AddSwaggerGen(options =>
        {
            // "Authorize" button: paste the accessToken returned by POST /api/auth/login
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Description = "accessToken returned by POST /api/auth/login"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        // Rate Limiting
        services.AddRateLimiter(options =>
        {
            // 100 request per minute (Globally by user identity or ip address) 
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                string? userId = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    return RateLimitPartition.GetFixedWindowLimiter<string>(
                        userId,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                }
                
                string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                
                return RateLimitPartition.GetFixedWindowLimiter<string>(
                    ip,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
            
            // Custom rejection handling logic (response)
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["X-RateLimit-Limit"] = "100";
                context.HttpContext.Response.Headers["X-RateLimit-Remaining"] ="0";
                context.HttpContext.Response.Headers["X-RateLimit-Reset"]= DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString(); 
                context.HttpContext.Response.Headers.RetryAfter = "60";
                context.HttpContext.Response.ContentType = "application/problem+json";

                ProblemDetails problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Rate limit exceeded",
                    Detail = "You have exceeded the allowed number of requests. Please wait before trying again.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
                    Instance = context.HttpContext.Request.Path
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };
        });
        
        // Exception Handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // Middlewares (Register factory-activated middleware if it has injected dependencies.)
        services.AddTransient<FallBackRouteMiddleware>();
        services.AddTransient<SessionValidationMiddleware>();
        
        // Cors
        services.AddCors(options =>
        {
            options.AddPolicy("default", policy =>
            {
                policy.SetIsOriginAllowed(origin => true);
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();                                
            });
        });
        
        return services;
    }
}