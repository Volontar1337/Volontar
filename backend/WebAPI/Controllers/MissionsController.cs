using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Organization")]
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;
        private readonly ILogger<MissionsController> _logger;

        public MissionsController(IMissionService missionService, ILogger<MissionsController> logger)
        {
            _missionService = missionService;
            _logger = logger;
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

        [Authorize(Roles = "Volunteer")]
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignToMission(Guid id)
        {
            var volunteerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (volunteerIdClaim == null)
                return Unauthorized("Missing volunteer ID.");

            var volunteerId = Guid.Parse(volunteerIdClaim.Value);

            var result = await _missionService.AssignVolunteerToMissionAsync(id, volunteerId);

            return result switch
            {
                AssignResult.Success => Ok("Successfully assigned to mission."),
                AssignResult.AlreadyAssigned => Conflict("Volunteer is already assigned."),
                AssignResult.MissionNotFound => NotFound("Mission not found."),
                AssignResult.VolunteerNotFound => NotFound("Volunteer not found."),
                _ => StatusCode(500, "An unexpected error occurred.")
            };
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        // Ny endpoint: Hämta volontärer som anmält sig till mission (Task 6)
        [HttpGet("{id}/assignments")]
        public async Task<IActionResult> GetAssignmentsForMission(Guid id)
        {
            var assignments = await _missionService.GetVolunteersForMissionAsync(id);
            return Ok(assignments);
        }
    }
}
