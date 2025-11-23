using System;
namespace AuthAPI.Application.Features.Auth.Common;

public record AuthResponse(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken
    );
