namespace AuthAPI.Application.Features.Auth.DTO;

public record LoginRequest(
    string Email, 
    string Password);
