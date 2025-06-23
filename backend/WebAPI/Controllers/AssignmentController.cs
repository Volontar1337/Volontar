using Application.Assignments;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssignmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Location) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                dto.Time == default)
            {
                return BadRequest("Alla fält måste vara ifyllda.");
            }

            var orgIdClaim = User.Claims.FirstOrDefault(c => c.Type == "OrganizationId");
            if (orgIdClaim == null)
                return Forbid("Du måste vara inloggad som organisation.");

            var assignment = new Assignment
            {
                OrganizationId = int.Parse(orgIdClaim.Value),
                Location = dto.Location,
                Time = dto.Time,
                Description = dto.Description
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Uppdrag skapat!", assignment.Id });
        }
    }
}
