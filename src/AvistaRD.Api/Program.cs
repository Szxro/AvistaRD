using AvistaRD.Api.Extensions;
using AvistaRD.Api.Middlewares;
using AvistaRD.Application;
using AvistaRD.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    // Load environment variables
    DotNetEnv.Env.Load();
    
    // Add environment variables to configuration
    builder.Configuration.AddEnvironmentVariables();

    // Add services to the container.
    builder.Services
        .AddInfrastructureLayer(builder.Environment)
        .AddApplicationLayer()
        .AddPresentationLayer(builder.Environment);
}

WebApplication app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    // Rejects the tokens of accounts deactivated or changed after the token was issued
    app.UseMiddleware<SessionValidationMiddleware>();

    app.UseAuthorization();

    app.MapControllers();
    
    app.UseMiddleware<FallBackRouteMiddleware>();

    app.Run();
}
