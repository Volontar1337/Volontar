using Application.Interfaces;
using Application.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Missing user ID.");

            var userId = Guid.Parse(userIdClaim.Value);

            // Skicka vidare till service
            var missionId = await _missionService.CreateMissionAsync(dto, userId);
            return Ok(new { id = missionId });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyMissions([FromQuery] MissionStatus? status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Missing user ID claim.");
                return Unauthorized("Missing user ID.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format: {UserIdValue}", userIdClaim.Value);
                return Unauthorized("Invalid user ID.");
            }

            _logger.LogInformation("Fetching missions for user ID: {UserId} with status filter: {Status}", userId, status);

            var missions = await _missionService.GetMissionsForUserAsync(userId, status);

            return Ok(missions);
        }


        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignToMission(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Missing user ID.");

            var userId = Guid.Parse(userIdClaim.Value);

            var result = await _missionService.AssignUserToMissionAsync(id, userId);

            return result switch
            {
                AssignResult.Success => Ok("Successfully assigned to mission."),
                AssignResult.AlreadyAssigned => Conflict("User is already assigned."),
                AssignResult.MissionNotFound => NotFound("Mission not found."),
                AssignResult.UserNotFound => NotFound("User not found."),
                _ => StatusCode(500, "An unexpected error occurred.")
            };
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [HttpGet("test-claims")]
        public IActionResult TestClaims()
        {
            return Ok("YES: claims endpoint works");
        }
        
        [HttpGet("{id}/assignments")]
        public async Task<IActionResult> GetAssignmentsForMission(Guid id)
        {
            var assignments = await _missionService.GetUsersForMissionAsync(id);
            return Ok(assignments);
        }
    }
}
