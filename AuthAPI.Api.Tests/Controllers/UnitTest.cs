using System.Net;

namespace AuthAPI.Api.Tests.Controllers;

public class UnitTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    const string ENDPOINT = "/api/weatherforecast";

    public UnitTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Run Migrations & Seed database.
        await context.Database.EnsureCreatedAsync();
        
        // Seed the Database here
        // await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up data between tests.
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task Method_Condition_Result()
    {
        // Act
        //var response = await _client.GetAsync(ENDPOINT);

        // Assert
        //response.EnsureSuccessStatusCode();
        //response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
