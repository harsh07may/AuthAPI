namespace AuthAPI.Application.Interfaces;

/// <summary>
/// Represents the application's EF Core database context surface used by the application layer.
/// </summary>
/// <remarks>
/// Allows us use EF Core directly within the Service layer and avoid Repository abstraction. <br /> 
/// Implement it in <b>Infrastructure/Persistence/AppDbContext.cs</b>.
/// </remarks>
public interface IAppDbContext
{
    //DbSet<WeatherForecast> WeatherForecasts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
