using AuthAPI.Application.Features.Auth.DTO; // NOTE: This is NOT Microsoft.AspNetCore.Identity.Data.RegisterRequest;
using Azure.Core;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest, CancellationToken ct)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerRequest, ct);
                return Ok(result);
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
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials.");
            }
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
    }
}
