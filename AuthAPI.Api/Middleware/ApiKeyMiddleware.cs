using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthAPI.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string API_KEY_HEADER = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
    {
        // If no key, let it pass. The [Authorize] attribute will catch it if it requires auth.
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            await _next(context);
            return;
        }

        // 2. Resolve DbContext (Scope is required because Middleware is Singleton)
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 3. Validate Key
        var apiKey = await dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == extractedApiKey.ToString() && k.IsActive);

        if (apiKey == null || apiKey.Expiration < DateTime.UtcNow)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid or Expired API Key");
            return;
        }

        // 4. Create Identity
        // We add a specific claim "AuthenticationType=ApiKey" so controllers know how they were logged in.
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, apiKey.Owner),
            new Claim(ClaimTypes.Role, "System"), // Example: Granting a "System" role
            new Claim("ApiKeyId", apiKey.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }
}