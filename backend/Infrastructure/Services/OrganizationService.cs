using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _context;

        public OrganizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrganizationProfile>> GetAllAsync()
        {
            return await _context.OrganizationProfiles
            .Include(o => o.User)
            .Include(o => o.Members)
                .ThenInclude(m => m.User)
            .ToListAsync();
        }

        public async Task<OrganizationProfile?> GetByIdAsync(Guid id)
        {
            return await _context.OrganizationProfiles.FindAsync(id);
        }

        public async Task<OrganizationProfile?> CreateAsync(Guid userId, OrganizationProfileDto dto)
        {
            var profile = new OrganizationProfile
            {
                UserId = userId,
                OrganizationName = dto.OrganizationName,
                ContactPerson = dto.ContactPerson,
                PhoneNumber = dto.PhoneNumber,
                Website = dto.Website,
                CreatedAt = DateTime.UtcNow
            };

            _context.OrganizationProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task<bool> ExistsAsync(Guid userId, string organizationName)
        {
            return await _context.OrganizationProfiles
                .AnyAsync(o => o.UserId == userId && o.OrganizationName == organizationName);
        }
    }
}
