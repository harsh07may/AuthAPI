namespace AuthAPI.Application.Interfaces;

public interface IAuthService
{

    Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
}
