using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/organizations")]
[Authorize]
public class OrganizationProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrganizationProfileController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganizationProfile([FromBody] OrganizationProfileDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        // Kontrollera att användaren inte redan har en OrganizationProfile
        var existing = await _context.OrganizationProfiles
            .AnyAsync(o => o.UserId == userId);
        if (existing)
            return Conflict(new { message = "User already has an organization profile." });

        var profile = new OrganizationProfile
        {
            UserId = userId,
            OrganizationName = dto.OrganizationName,
            ContactPerson = dto.ContactPerson,
            PhoneNumber = dto.PhoneNumber,
            Website = dto.Website,
            CreatedAt = DateTime.UtcNow
        };

        _context.OrganizationProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var profile = await _context.OrganizationProfiles.FindAsync(id);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }
}
