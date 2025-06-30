using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMissionService _missionService;

        public AssignmentController(AppDbContext context, IMissionService missionService)
        {
            _context = context;
            _missionService = missionService;
        }

        /*[HttpPost]
        //[AllowAnonymous]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> CreateAssignment([FromBody] MissionAssignmentCreateDto dto)
        {
            // Kontroll: Finns både mission och volontär?
            var missionExists = await _context.Missions.AnyAsync(m => m.Id == dto.MissionId);
            var volunteerExists = await _context.Users.AnyAsync(u => u.Id == dto.VolunteerId && u.Role == Domain.Enums.UserRole.Volunteer);

            if (!missionExists || !volunteerExists)
                return NotFound("Uppdrag eller volontär hittades inte.");

            // Skapa ny MissionAssignment
            var assignment = new MissionAssignment
            {
                MissionId = dto.MissionId,
                VolunteerId = dto.VolunteerId,
                AssignedAt = DateTime.UtcNow,
                RoleDescription = dto.RoleDescription
            };

            _context.MissionAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Du har anmält dig till uppdraget!", assignment.Id });
        }*/

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _context.MissionAssignments
                .Include(a => a.Mission)
                .Include(a => a.Volunteer)
                .OrderBy(a => a.AssignedAt)
                .Select(ma => new
                {
                    ma.Id,
                    MissionTitle = ma.Mission.Title,
                    VolunteerName = ma.Volunteer.FirstName + " " + ma.Volunteer.LastName,
                    VolunteerEmail = ma.Volunteer.User.Email,
                    AssignedAt = ma.AssignedAt,
                    Role = ma.RoleDescription
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
        [HttpGet("test-seed")]
        [AllowAnonymous]
        public async Task<IActionResult> TestSeed()
        {
            var result = await _context.MissionAssignments
                .Include(ma => ma.Mission)
                .Include(ma => ma.Volunteer)
                .Select(ma => new
                {
                    ma.Id,
                    MissionId = ma.MissionId,
                    ma.Mission.Title,
                    VolunteerId = ma.VolunteerId,
                    VolunteerName = ma.Volunteer.FirstName + " " + ma.Volunteer.LastName,
                    ma.RoleDescription,
                    ma.AssignedAt
                })
                .ToListAsync();

            return Ok(result);
        }
        [HttpPost("assign")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> AssignVolunteer([FromBody] AssignVolunteerDto dto)
        {
            var success = await _missionService.AssignVolunteerToMissionAsync(dto.MissionId, dto.VolunteerId);

            if (!success)
                return BadRequest("Volunteer is already assigned to this mission.");

            return Ok("Volunteer successfully assigned to mission.");
        }
    }
}
