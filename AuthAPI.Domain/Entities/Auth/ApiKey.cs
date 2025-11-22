namespace AuthAPI.Domain.Entities.Auth;

public class ApiKey : BaseEntity<Guid>
{
    public string Key { get; set; } = string.Empty; // The actual secret key
    public string Owner { get; set; } = string.Empty; // Name of the service/client
    public DateTime Expiration { get; set; }
    public bool IsActive { get; set; } = true;

    // Optional: Define specific claims/scopes this key allows
    // public List<string> Permissions { get; set; } = new(); 
}