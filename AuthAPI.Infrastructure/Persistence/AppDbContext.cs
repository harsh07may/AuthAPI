using AuthAPI.Domain.Entities.Auth;

namespace AuthAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options) 
    {
        _currentUserService = currentUserService;
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Configure Many-to-Many for UserRole
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(u => u.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;
        var timestamp = DateTime.UtcNow;

        // 1. Detect Changes & snapshot the entries immediately
        var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

        // 2. Create Audit Records
        foreach (var entry in entries)
        {
            // This breaks when DB-side NEWSEQUENTIALID()
            var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString();

            AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = entry.State.ToString(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entityId,
                Timestamp = timestamp
            });
        }

        // 3. Save everything (Original changes + Audit logs) in one transaction
        return await base.SaveChangesAsync(cancellationToken);
    }
}
