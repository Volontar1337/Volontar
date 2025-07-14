using Application.DTOs;
using Application.Interfaces;
using Domain.Enums; // ← För AssignResult
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
                .Include(a => a.User)
                .OrderBy(a => a.AssignedAt)
                .Select(ma => new
                {
                    ma.Id,
                    MissionTitle = ma.Mission.Title,
                    UserName = ma.User.FirstName + " " + ma.User.LastName,
                    UserEmail = ma.User.Email,
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
                .Include(ma => ma.User)
                .Select(ma => new
                {
                    ma.Id,
                    MissionId = ma.MissionId,
                    ma.Mission.Title,
                    UserId = ma.UserId,
                    UserName = ma.User.FirstName + " " + ma.User.LastName,
                    ma.RoleDescription,
                    ma.AssignedAt
                })
                .ToListAsync();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("assign")]
        //[Authorize] // Behåll Authorize om du vill att användare måste vara inloggade för att tilldela
        public async Task<IActionResult> AssignUser([FromBody] AssignMissionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Could not extract user ID from token.");
            }

            var result = await _missionService.AssignUserToMissionAsync(dto.MissionId, userId);

            return result switch
            {
                AssignResult.Success => Ok("User successfully assigned to mission."),
                AssignResult.AlreadyAssigned => Conflict("User already assigned."),
                AssignResult.MissionNotFound => NotFound("Mission not found."),
                AssignResult.UserNotFound => NotFound("User not found."),
                _ => StatusCode(500, "Unexpected error occurred.")
            };
        }
    }
}
