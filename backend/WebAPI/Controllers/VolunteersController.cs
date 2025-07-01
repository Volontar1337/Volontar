using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolunteersController : ControllerBase
{
    private readonly AppDbContext _context;

    public VolunteersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}/missions")]
    public async Task<IActionResult> GetMissionsForVolunteer(Guid id)
    {
        var volunteerExists = await _context.Volunteers.AnyAsync(v => v.Id == id);
        if (!volunteerExists)
        {
            return NotFound(new { message = $"No volunteer found with ID {id}" });
        }

        var missions = await _context.MissionAssignments
            .Where(ma => ma.VolunteerId == id)
            .Select(ma => new
            {
                ma.Mission.Id,
                ma.Mission.Title,
                ma.Mission.Status,
                ma.AssignedAt
            })
            .ToListAsync();

        return Ok(missions);
    }
}
