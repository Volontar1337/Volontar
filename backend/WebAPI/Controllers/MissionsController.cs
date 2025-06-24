using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Organization")]
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
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
        public async Task<IActionResult> GetMyMissions()
        {
            /*var orgIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (orgIdClaim == null)
                return Unauthorized("Missing organization ID.");

            var orgId = Guid.Parse(orgIdClaim.Value);*/

            var orgId = Guid.Parse("235CAD2A-F261-4854-8EFF-220E1A6BB04B");

            var missions = await _missionService.GetMissionsByOrganizationIdAsync(orgId);
            return Ok(missions);
        }
    }
}
