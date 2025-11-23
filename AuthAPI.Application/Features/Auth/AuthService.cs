using AuthAPI.Application.Features.Auth.DTO;
using AuthAPI.Domain.Entities.Auth;

namespace AuthAPI.Application.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    public AuthService(
        IAppDbContext context, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        
        // 1. Validate
        if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken))
        {
            throw new InvalidOperationException("User already exists.");
        }

        // 2.Create User
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerRequest.Email,
            PasswordHash = _passwordHasher.Hash(registerRequest.Password)
        };

        // 3. Generate Tokens & Save to Db
        var accessToken = _jwtProvider.GenerateToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email, accessToken, refreshToken.Token);

    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        // 1. Validate
        var user = await _context.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.Email == loginRequest.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(loginRequest.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // 2. Generate Tokens & Save to Db
        var accessToken = _jwtProvider.GenerateToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();
        
        user.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email, accessToken, refreshToken.Token);
    }

    public async Task<bool> CreateRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == roleName, cancellationToken))
        {
            throw new InvalidOperationException($"Role '{roleName}' already exists.");
        }

        var role = new Role { Id = Guid.NewGuid(), Name = roleName };
        _context.Roles.Add(role);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AssignRoleAsync(string userEmail, string roleName, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userEmail, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User '{userEmail}' not found.");

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);

        if (role == null)
            throw new KeyNotFoundException($"Role '{roleName}' not found.");

        var exists = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);

        if (exists)
            throw new InvalidOperationException("User is already assigned to this role.");

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

}
