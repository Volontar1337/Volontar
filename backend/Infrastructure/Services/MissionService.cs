using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class MissionService : IMissionService
    {
        private readonly AppDbContext _context;

        public MissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MissionDto>> GetAllAsync()
        {
            var missions = await _context.Missions.ToListAsync();
            return missions.Select(m => new MissionDto
            {
                Id          = m.Id,
                Title       = m.Title,
                Description = m.Description,
                Location    = m.Location,
                StartTime   = m.StartTime,
                EndTime     = m.EndTime,
                Status      = m.Status.ToString()
            });
        }

        public async Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId, MissionStatus? status = null)
        {
            var query = _context.Missions.Where(m => m.CreatedByOrgId == organizationId);
            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            var missions = await query.ToListAsync();
            return missions.Select(m => new MissionDto
            {
                Id          = m.Id,
                Title       = m.Title,
                Description = m.Description,
                Location    = m.Location,
                StartTime   = m.StartTime,
                EndTime     = m.EndTime,
                Status      = m.Status.ToString()
            });
        }

        public async Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationUserId)
        {
             // 1) Hämta organisationens profil via UserId
             var orgProfile = await _context.OrganizationProfiles
              .SingleOrDefaultAsync(p => p.UserId == organizationUserId);
             if (orgProfile == null)
                throw new InvalidOperationException("Organization profile not found.");

            // 2) Skapa mission med rätt FK (profilens Id)
             var mission = new Mission
             {
                Id               = Guid.NewGuid(),
                Title            = dto.Title,
                Description      = dto.Description,
                Location         = dto.Location,
                StartTime        = dto.StartTime,
                EndTime          = dto.EndTime,
                CreatedByOrgId   = orgProfile.Id
            };

                _context.Missions.Add(mission);
                await _context.SaveChangesAsync();
                return mission.Id;
        }
        public async Task<AssignResult> AssignVolunteerToMissionAsync(Guid missionId, Guid volunteerId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null) return AssignResult.MissionNotFound;

            var volunteerProfile = await _context.VolunteerProfiles.FindAsync(volunteerId);
            if (volunteerProfile == null) return AssignResult.VolunteerNotFound;

            var already = await _context.MissionAssignments
                .AnyAsync(a => a.MissionId == missionId && a.VolunteerId == volunteerId);
            if (already) return AssignResult.AlreadyAssigned;

            _context.MissionAssignments.Add(new MissionAssignment
            {
                MissionId   = missionId,
                VolunteerId = volunteerId,
                AssignedAt  = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return AssignResult.Success;
        }

        public async Task<List<VolunteerDto>> GetVolunteersForMissionAsync(Guid missionId)
        {
            return await _context.MissionAssignments
                .Where(ma => ma.MissionId == missionId)
                .Include(ma => ma.Volunteer)
                    .ThenInclude(v => v.User)
                .Select(ma => new VolunteerDto
                {
                    Id        = ma.Volunteer.Id,
                    FirstName = ma.Volunteer.FirstName,
                    LastName  = ma.Volunteer.LastName,
                    Email     = ma.Volunteer.User.Email
                })
                .ToListAsync();
        }
    }
}
