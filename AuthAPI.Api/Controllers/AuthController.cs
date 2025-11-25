using AuthAPI.Application.Features.Auth.DTO; // NOTE: This is NOT Microsoft.AspNetCore.Identity.Data.RegisterRequest;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AuthAPI.Application.Features.Auth.DTO.RoleRequest;

namespace AuthAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var name = User.Identity?.Name;

            return Ok(new
            {
                Email = email,
                Name = name,
                Role = role
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest, CancellationToken ct)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerRequest, ct);
                SetRefreshTokenCookie(result.RefreshToken);
                
                return Ok(new 
                {
                    result.Id,
                    result.Email,
                    result.AccessToken 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken ct)
        {
            try
            {
                var result = await _authService.LoginAsync(loginRequest, ct);
                SetRefreshTokenCookie(result.RefreshToken);

                return Ok(new 
                { 
                    result.Id, 
                    result.Email, 
                    result.AccessToken 
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials.");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No Refresh Token provided.");

            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken, ct);
                SetRefreshTokenCookie(result.RefreshToken);

                return Ok(new { result.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid token.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken, ct);
            }

            // Delete Cookie
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out.");
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken ct)
        {
            try
            {
                await _authService.CreateRoleAsync(request.RoleName, ct);
                return Ok($"Role '{request.RoleName}' created successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("roles/assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken ct)
        {
            try
            {
                await _authService.AssignRoleAsync(request.UserEmail, request.RoleName, ct);
                return Ok($"Role '{request.RoleName}' assigned to {request.UserEmail}.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("api-keys")]
        public async Task<IActionResult> CreateApiKey(string owner, CancellationToken ct)
        {
            try
            {
                var apiKey = await _authService.GenerateApiKeyAsync(owner, ct);
                return Ok(new { apiKey.Key, apiKey.Owner, apiKey.Expiration });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Helper to set refresh-token as HttpOnly Cookie.
        /// </summary>
        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
