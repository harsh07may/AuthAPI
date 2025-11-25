using AuthAPI.Domain.Entities;
using AuthAPI.Domain.Entities.Auth;

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

    public DbSet<AuditLog> AuditLogs { get; }
    public DbSet<User> Users {get;}
    public DbSet<Role> Roles {get;}
    public DbSet<UserRole> UserRoles {get;}
    public DbSet<RefreshToken> RefreshTokens {get;}
    public DbSet<ApiKey> ApiKeys { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
