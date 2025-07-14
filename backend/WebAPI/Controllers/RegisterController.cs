using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public RegisterController(IUserService userService, AppDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email is already in use." });

        try
        {
            var result = await _userService.RegisterUserAsync(dto);
            return Created(string.Empty, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
        }
    }
}
