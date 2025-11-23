namespace AuthAPI.Application.Features.Auth.DTO;

public class RoleRequest
{
    public record CreateRoleRequest(string RoleName);
    public record AssignRoleRequest(string UserEmail, string RoleName);
}
