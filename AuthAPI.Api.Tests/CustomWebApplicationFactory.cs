namespace AuthAPI.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return _msSqlContainer.StartAsync();
    }
    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = _msSqlContainer.GetConnectionString();
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = "IntegrationTestsDb"
                };

                options.UseSqlServer(builder.ConnectionString);
            });
        });
    }

}
