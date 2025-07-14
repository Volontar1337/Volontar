using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationMembersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizationMembersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetMembers(Guid organizationId)
        {
            var members = await _context.OrganizationMembers
                .Where(m => m.OrganizationProfileId == organizationId)
                .Include(m => m.User)
                .Select(m => new
                {
                    m.Id,
                    m.Role,
                    m.JoinedAt,
                    User = new
                    {
                        m.User.Id,
                        m.User.FirstName,
                        m.User.LastName,
                        m.User.Email
                    }
                })
                .ToListAsync();

            return Ok(members);
        }
    }
}
