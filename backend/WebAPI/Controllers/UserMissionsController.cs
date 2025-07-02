using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/users/{id}/missions")]
public class UserMissionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserMissionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMissionsForUser(Guid id)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == id);
        if (!userExists)
        {
            return NotFound(new { message = $"No user found with ID {id}" });
        }

        var missions = await _context.MissionAssignments
            .Include(ma => ma.Mission)
            .Where(ma => ma.UserId == id)
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
