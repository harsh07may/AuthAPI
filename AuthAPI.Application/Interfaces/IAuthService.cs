namespace AuthAPI.Application.Interfaces;

public interface IAuthService
{

    Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<bool> CreateRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<bool> AssignRoleAsync(string userEmail, string roleName, CancellationToken cancellationToken);
    Task<AuthResponse> RefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task RevokeTokenAsync(string token, CancellationToken cancellationToken);
}
