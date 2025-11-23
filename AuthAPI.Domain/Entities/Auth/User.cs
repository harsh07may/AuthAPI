namespace AuthAPI.Domain.Entities.Auth;

public class User : BaseEntity<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public List<UserRole> UserRoles { get; set; } = new();
}