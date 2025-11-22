namespace AuthAPI.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
}
