using Serilog;
using Serilog.Events;

namespace AuthAPI.Api.Configuration
{
    public class LoggingConfiguration
    {
        public static void ConfigureSerilog(IConfiguration configuration, IHostEnvironment env)
        {
            var config = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "CleanArchitectureAPI")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);

            // ONLY log SQL queries in Development
            if (env.IsDevelopment())
            {
                config.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information);
            }
            else
            {
                config.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            }

            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            Log.Logger = config.CreateLogger();
        }
    }
}
