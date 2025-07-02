using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService  _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService  = userService;
            _tokenService = tokenService;
        }

        // ── LOGIN ─────────────────────────────────────────────────────
        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            // Anropa din AuthenticateAsync-metod
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Skapa JWT
            var token = _tokenService.CreateToken(user);

            // Returnera token
            return Ok(new LoginResponseDto { Token = token });
        }

        // ── REGISTER VOLUNTEER ────────────────────────────────────────
        [HttpPost("register-volunteer"), AllowAnonymous]
        public async Task<IActionResult> RegisterVolunteer([FromBody] RegisterVolunteerRequestDto dto)
        {
            // Skapa en ny volunteer via din UserService
            var result = await _userService.RegisterVolunteerAsync(dto);

            // Bygg en User-instans med det nya ID:t för token-issuing
            var user  = new User { Id = result.UserId, Email = dto.Email };
            var token = _tokenService.CreateToken(user);

            return Created(string.Empty, new RegisterResponseDto
            {
                UserId = result.UserId,
                Token  = token
            });
        }

        // ── REGISTER ORGANIZATION ─────────────────────────────────────
        [HttpPost("register-organization"), AllowAnonymous]
        public async Task<IActionResult> RegisterOrganization([FromBody] RegisterOrganizationRequestDto dto)
        {
            var result = await _userService.RegisterOrganizationAsync(dto);
            var user   = new User { Id = result.UserId, Email = dto.Email };
            var token  = _tokenService.CreateToken(user);

            return Created(string.Empty, new RegisterResponseDto
            {
                UserId = result.UserId,
                Token  = token
            });
        }
    }
}
