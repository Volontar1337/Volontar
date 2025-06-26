using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        // Välj ett tydligt namn för authentication-schemat, exempelvis "CookieAuth"
        private const string AuthScheme = "CookieAuth";

        public AuthController(ILogger<AuthController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Endast för test/dev! Loggar in som en mock-organisation.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login-test")]
        public async Task<IActionResult> LoginTest()
        {
            var mockOrgId = "11111111-1111-1111-1111-111111111111";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, mockOrgId),
                new Claim(ClaimTypes.Role, "Organization"),
                new Claim(ClaimTypes.Name, "Mock Organization"),
                new Claim("OrganizationId", mockOrgId) // Gör OrganizationId tillgängligt direkt
            };

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Tydlig expiration
            };

            await HttpContext.SignInAsync(AuthScheme, principal, authProperties);

            _logger.LogInformation("Mock login issued for OrgId: {OrgId}", mockOrgId);

            return Ok(new { message = "Mock login successful" });
        }

        /// <summary>
        /// Riktig inloggning med credentials.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Autentisera användaren via UserService
            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                // Säg ALDRIG vilket fält som är fel av säkerhetsskäl!
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Lägg till OrganizationId-claim om det är en organisation
            if (user.Role.ToString() == "Organization")
            {
                claims.Add(new Claim("OrganizationId", user.Id.ToString()));
            }

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Samma som test
            };

            await HttpContext.SignInAsync(AuthScheme, principal, authProperties);

            _logger.LogInformation("Login successful for user {UserId} ({Email}) with role {Role}", user.Id, user.Email, user.Role);

            // Skicka aldrig med lösenord eller känslig info!
            var response = new LoginResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return Ok(response);
        }

        /// <summary>
        /// Logga ut den inloggade användaren.
        /// </summary>
        [Authorize(AuthenticationSchemes = AuthScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AuthScheme);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}