using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Auth;
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

    [HttpPost("volunteer")]
    public async Task<IActionResult> RegisterVolunteer([FromBody] RegisterVolunteerRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email is already in use." });

        try
        {
            var result = await _userService.RegisterVolunteerAsync(dto);
            return Created(string.Empty, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
        }
    }

    [HttpPost("organization")]
    public async Task<IActionResult> RegisterOrganization([FromBody] RegisterOrganizationRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email is already in use." });

        try
        {
            var result = await _userService.RegisterOrganizationAsync(dto);
            return Created(string.Empty, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
        }
    }
}
