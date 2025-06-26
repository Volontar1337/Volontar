using Application.Assignments;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            // Kontrollera indata
            if (string.IsNullOrWhiteSpace(dto.Location) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                dto.Time == default)
            {
                return BadRequest("Alla fält måste vara ifyllda.");
            }

            // Hämta OrganizationId från claims
            var orgIdClaim = User.Claims.FirstOrDefault(c => c.Type == "OrganizationId");
            if (orgIdClaim == null)
                return StatusCode(403, "Du måste vara inloggad som organisation.");

            // Om OrganizationId är string/GUID i modellen:
            var assignment = new Assignment
            {
                OrganizationId = orgIdClaim.Value,
                Location = dto.Location,
                Time = dto.Time,
                Description = dto.Description
            };

            // Om OrganizationId är int, avkommentera nedan istället:
            // var assignment = new Assignment
            // {
            //     OrganizationId = int.Parse(orgIdClaim.Value),
            //     ...
            // };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Uppdrag skapat!", assignment.Id });
        }

        // Endast för utveckling/felsökning
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}