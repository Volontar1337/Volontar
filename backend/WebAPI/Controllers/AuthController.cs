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
            // This is only for test/dev!
            var mockOrgId = "11111111-1111-1111-1111-111111111111";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, mockOrgId),
                new Claim(ClaimTypes.Role, "Organization"),
                new Claim(ClaimTypes.Name, "Mock Organization")
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync("Cookies", principal, authProperties);

            _logger.LogInformation("Mock login issued for OrgId: {OrgId}", mockOrgId);

            return Ok(new { message = "Mock login successful" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Validate credentials using your real user service
            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                // Never reveal which field was wrong (security best practice)
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Build claims for authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Persistent cookie/session
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Optional: Set expiry (should match Program.cs)
            };

            // Sign in: this issues the cookie!
            await HttpContext.SignInAsync("Cookies", principal, authProperties);

            _logger.LogInformation("Login successful for user {UserId}", user.Id);

            // Return basic user info (never sensitive data)
            var response = new LoginResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return Ok(response); // Cookie will be sent automatically in Set-Cookie header
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
