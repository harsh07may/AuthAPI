namespace AuthAPI.Domain.Entities.Auth;

public class RefreshToken : BaseEntity<Guid>
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
