using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        [HttpGet]
        [Authorize(Roles = "Organization,Volunteer")]
        public async Task<IActionResult> GetAllMissions()
        {
            var missions = await _missionService.GetAllAsync();
            return Ok(missions);
        }

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> CreateMission([FromBody] CreateMissionDto dto)
        {
            var orgIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (orgIdClaim == null) return Unauthorized("Missing organization ID.");
            var orgId = Guid.Parse(orgIdClaim.Value);
            var missionId = await _missionService.CreateMissionAsync(dto, orgId);
            return Ok(new { id = missionId });
        }

        [HttpGet("my")]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> GetMyMissions([FromQuery] MissionStatus? status)
        {
            var orgIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (orgIdClaim == null) return Unauthorized("Missing organization ID.");
            var orgId = Guid.Parse(orgIdClaim.Value);
            var missions = await _missionService.GetMissionsByOrganizationIdAsync(orgId, status);
            return Ok(missions);
        }

        [HttpGet("claims")]
        [Authorize]
        public IActionResult Claims()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();
            return Ok(claims);
        }

        [HttpPost("{id}/assign")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> AssignToMission(Guid id)
        {
            var volClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (volClaim == null) return Unauthorized("Missing volunteer ID.");
            var volunteerId = Guid.Parse(volClaim.Value);
            var result = await _missionService.AssignVolunteerToMissionAsync(id, volunteerId);
            return result switch
            {
                AssignResult.Success           => Ok("Assigned successfully."),
                AssignResult.AlreadyAssigned   => Conflict("Already assigned."),
                AssignResult.MissionNotFound   => NotFound("Mission not found."),
                AssignResult.VolunteerNotFound => NotFound("Volunteer not found."),
                _                              => StatusCode(500, "Unexpected error.")
            };
        }

        [HttpGet("{id}/assignments")]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> GetAssignmentsForMission(Guid id)
        {
            var assignments = await _missionService.GetVolunteersForMissionAsync(id);
            return Ok(assignments);
        }
    }
}