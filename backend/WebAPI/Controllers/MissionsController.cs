using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Enums;
using Infrastructure.Persistence;       // <-- Viktigt!
using Microsoft.EntityFrameworkCore;    // <-- Viktigt!

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Organization")]
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;
        private readonly ILogger<MissionsController> _logger;
        private readonly AppDbContext _context;    // <-- Lägg till

        public MissionsController(
            IMissionService missionService,
            ILogger<MissionsController> logger,
            AppDbContext context)                 // <-- Lägg till
        {
            _missionService = missionService;
            _logger = logger;
            _context = context;                   // <-- Lägg till
        }

        [HttpPost]
        public async Task<IActionResult> CreateMission([FromBody] CreateMissionDto dto)
        {
            var orgIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (orgIdClaim == null)
                return Unauthorized("Missing organization ID.");

            var orgId = Guid.Parse(orgIdClaim.Value);

            var missionId = await _missionService.CreateMissionAsync(dto, orgId);
            return Ok(new { id = missionId });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyMissions([FromQuery] MissionStatus? status)
        {
            var orgIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (orgIdClaim == null)
            {
                _logger.LogWarning("Missing organization ID claim.");
                return Unauthorized("Missing organization ID.");
            }

            if (!Guid.TryParse(orgIdClaim.Value, out Guid orgId))
            {
                _logger.LogWarning("Invalid organization ID format: {OrgIdValue}", orgIdClaim.Value);
                return Unauthorized("Invalid organization ID.");
            }

            _logger.LogInformation("Fetching missions for organization ID: {OrganizationId} with status filter: {Status}", orgId, status);

            var missions = await _missionService.GetMissionsByOrganizationIdAsync(orgId, status);

            return Ok(missions);
        }

        // Här är endpointen du ska ha med!
        [HttpGet("{id}/assignments")]       
        public async Task<IActionResult> GetAssignmentsForMission(Guid id)
        {
            var assignments = await _context.MissionAssignments
                .Include(ma => ma.Volunteer)
                .Where(ma => ma.MissionId == id)
                .Select(ma => new
                {
                    ma.Id,
                    ma.VolunteerId,
                    VolunteerName = ma.Volunteer.FirstName + " " + ma.Volunteer.LastName,
                    ma.AssignedAt,
                    ma.RoleDescription
                })
                .ToListAsync();

            return Ok(assignments);
        }
    }
}
