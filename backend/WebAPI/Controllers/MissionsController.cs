
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Kräver JWT
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;
        private readonly ILogger<MissionsController> _logger;

        public MissionsController(IMissionService missionService, ILogger<MissionsController> logger)
        {
            _missionService = missionService;
            _logger = logger;
        }

        /// <summary>
        /// Hämtar missions som har skapats av den inloggade användaren.
        /// </summary>
        [HttpGet("my-created")]
        public async Task<IActionResult> GetMyCreatedMissions([FromQuery] MissionStatus? status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Missing user ID claim.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid user ID format.");

            var missions = await _missionService.GetMissionsForUserAsync(userId, status);
            return Ok(missions);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMission([FromBody] CreateMissionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Missing user ID.");

            var userId = Guid.Parse(userIdClaim.Value);
            var missionId = await _missionService.CreateMissionAsync(dto, userId);

            return Ok(new { id = missionId });
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
                AssignResult.AlreadyAssigned => Conflict("Already assigned."),
                AssignResult.MissionNotFound => NotFound("Mission not found."),
                AssignResult.UserNotFound => NotFound("User not found."),
                _ => StatusCode(500, "Unexpected error.")
            };
        }

        [HttpGet("{id}/assignments")]
        public async Task<IActionResult> GetAssignmentsForMission(Guid id)
        {
            var assignments = await _missionService.GetUsersForMissionAsync(id);
            return Ok(assignments);
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}
