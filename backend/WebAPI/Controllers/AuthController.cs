using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login-test")]
        public async Task<IActionResult> LoginTest()
        {
            var mockOrgId = "11111111-1111-1111-1111-111111111111";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, mockOrgId),
                new Claim(ClaimTypes.Role, "Organization"),
                new Claim(ClaimTypes.Name, "Mock Organization")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync("CookieAuth", principal, authProperties);

            _logger.LogInformation("Mock login issued for OrgId: {OrgId}", mockOrgId);

            return Ok("Mock login successful");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Use the real UserService for login
            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                // Generic error for security reasons
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Optionally: You could sign in with CookieAuth here already
            // (That comes in next SYS-step, for now just return user info)

            var response = new LoginResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return Ok("Logged out successfully");
        }
    }
}
