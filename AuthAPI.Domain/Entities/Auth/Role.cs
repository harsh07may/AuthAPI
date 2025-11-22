namespace AuthAPI.Domain.Entities.Auth;

public class Role : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
