using AuthAPI.Domain.Entities.Auth;

namespace AuthAPI.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
    RefreshToken GenerateRefreshToken();
}