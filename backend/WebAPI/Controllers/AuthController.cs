using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // ── LOGIN ─────────────────────────────────────────────────────
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var token = _tokenService.CreateToken(user);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                FirstName = user.FirstName,    // <-- Lägg till
                LastName = user.LastName,      // <-- Lägg till
                Role = user.Role
            };

            return Ok(response);
        }

        // ── REGISTER ──────────────────────────────────────────────────
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto dto)
        {
            var result = await _userService.RegisterUserAsync(dto);
            var user = new User { Id = result.UserId, Email = dto.Email };
            var token = _tokenService.CreateToken(user);

            return Created(string.Empty, new RegisterResponseDto
            {
                UserId = result.UserId,
                Token = token
            });
        }

        // ── EXEMPEL PÅ SKYDDAD ROUTE ──────────────────────────────────
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Hämta user från databas via service (t.ex. med Guid)
            var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound();

            return Ok(new
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            });
        }
    }
}
