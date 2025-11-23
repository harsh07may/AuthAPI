using Serilog;
using System.Text;
using AuthAPI.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;


var app = CreateWebApplication(args);
await ConfigureAndRunApp(app);

WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    LoggingConfiguration.ConfigureSerilog(builder.Configuration, builder.Environment);
    builder.Host.UseSerilog();

    // Health checks
    builder.Services.AddHealthChecks()
        .AddCheck<LivenessHealthCheck>("self")
        .AddCheck<ReadinessHealthCheck>("readiness");

    // Auth services
    var jwtConfig = builder.Configuration.GetSection("Jwt");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]!))
        };
    });
    builder.Services.AddAuthorization();

    // Register HttpContextAccessor for accessing User Claims.
    builder.Services.AddHttpContextAccessor();

    // Add services to the container.
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddOpenApi();

    return builder.Build();
}
Task ConfigureAndRunApp(WebApplication app)
{
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("liveness"),
    });

    app.MapHealthChecks("/ready", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("readiness"),
    });

    return app.RunAsync();
}
