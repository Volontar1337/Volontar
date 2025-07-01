using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpPost("{missionId}/assign")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> AssignVolunteer(Guid missionId, [FromBody] AssignMissionRequestDto dto)
        {
            var success = await _missionService.AssignVolunteerToMissionAsync(missionId, dto.VolunteerId);

            if (!success)
                return Conflict("Volunteer is already assigned to this mission.");

            return Ok("Volunteer successfully assigned to mission.");
        }
    } 
}
