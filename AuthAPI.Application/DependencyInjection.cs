
// We need this package for IServiceCollection
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        return services;
    }
}
