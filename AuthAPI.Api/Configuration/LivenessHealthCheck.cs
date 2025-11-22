using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuthAPI.Api.Configuration;

public class LivenessHealthCheck : IHealthCheck
{
    // Add any quick in-process checks here (e.g., memory thresholds) if needed.
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
           => Task.FromResult(HealthCheckResult.Healthy("Application is running."));
}
