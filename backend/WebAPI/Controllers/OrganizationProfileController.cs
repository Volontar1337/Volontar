using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    [Authorize]
    public class OrganizationProfileController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationProfileController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var organizations = await _organizationService.GetAllAsync();
            return Ok(organizations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganizationProfile([FromBody] OrganizationProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            // Kontrollera om organisationen redan finns för denna användare
            var exists = await _organizationService.ExistsAsync(userId, dto.OrganizationName);
            if (exists)
                return Conflict(new { message = "You already have an organization with this name." });

            var profile = await _organizationService.CreateAsync(userId, dto);
            if (profile == null)
                return BadRequest("Failed to create organization profile.");

            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var profile = await _organizationService.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
    }
}