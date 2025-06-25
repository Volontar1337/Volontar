using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Auth;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Here we just add a placeholder, we will implement real validation in SYS-02.2!
            if (loginDto.Email == "test@test.com" && loginDto.Password == "Password123!")
            {
                var response = new LoginResponseDto
                {
                    UserId = "1",
                    Email = loginDto.Email,
                    Role = "Organization" // or "Volunteer", depending on the logged-in type
                };
                return Ok(response);
            }
            else
            {
                // NOTE: Generic error message for security
                return Unauthorized(new { message = "Invalid email or password." });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return Ok("Logged out successfully");
        }
    }
}
