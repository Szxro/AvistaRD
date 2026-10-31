using AvistaRD.Infrastructure;
using Microsoft.AspNetCore.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    // Load environment variables
    DotNetEnv.Env.Load();
    
    // Add environment variables to configuration
    builder.Configuration.AddEnvironmentVariables();

    // Add services to the container.
    builder.Services.AddInfrastructureLayer(builder.Environment);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
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
    
    app.UseAuthorization();

    app.MapGroup("/api/auth").MapIdentityApi<IdentityUser>();

    app.MapControllers();

    app.Run();
}
