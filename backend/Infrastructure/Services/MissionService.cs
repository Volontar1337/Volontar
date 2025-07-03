using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class MissionService : IMissionService
    {
        private readonly AppDbContext _context;

        public MissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MissionDto>> GetMissionsForUserAsync(Guid userId, MissionStatus? status = null)
        {
            var missions = await _context.Missions
                .Where(m => m.CreatedByUserId == userId)
                .ToListAsync();

            if (status.HasValue)
            {
                missions = missions
                    .Where(m => m.Status == status.Value)
                    .ToList();
            }

            return missions.Select(m => new MissionDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Location = m.Location,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Status = m.Status.ToString()
            });
        }

        public async Task<IEnumerable<MissionDto>> GetAllAsync()
        {
            var missions = await _context.Missions.ToListAsync();

            return missions.Select(m => new MissionDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Location = m.Location,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Status = m.Status.ToString()
            });
        }

        public async Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId, MissionStatus? status = null)
        {
            var missions = await _context.Missions
                .Where(m => m.CreatedByOrgId == organizationId)
                .ToListAsync();

            if (status.HasValue)
            {
                missions = missions
                    .Where(m => m.Status == status.Value)
                    .ToList();
            }

            return missions.Select(m => new MissionDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Location = m.Location,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Status = m.Status.ToString()
            });
        }

        public async Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            OrganizationProfile? orgProfile = null;

            // Om OrganizationId är angivet, hämta organisationen och validera att user är owner/admin
            if (dto.OrganizationId.HasValue)
            {
                orgProfile = await _context.OrganizationProfiles
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == dto.OrganizationId.Value);

                if (orgProfile == null)
                    throw new InvalidOperationException("Organization not found.");

                // Enkel admin-koll: användaren måste vara owner (kopplad till org)
                if (orgProfile.UserId != userId)
                    throw new UnauthorizedAccessException("You do not have permission to create a mission for this organization.");
            }
            else
            {
                // Skapa privat mission, organizationProfile lämnas som null
                orgProfile = null;
            }

            var mission = new Mission
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedByUserId = userId,
                CreatedByUser = user,
                CreatedByOrgId = orgProfile?.Id,       // Kan vara null!
                CreatedByOrg = orgProfile              // Kan vara null!
            };

            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            return mission.Id;
        }

        public async Task<AssignResult> AssignUserToMissionAsync(Guid missionId, Guid userId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
                return AssignResult.MissionNotFound;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return AssignResult.UserNotFound;

            var alreadyAssigned = await _context.MissionAssignments
                .AnyAsync(ma => ma.MissionId == missionId && ma.UserId == userId);

            if (alreadyAssigned)
                return AssignResult.AlreadyAssigned;

            _context.MissionAssignments.Add(new MissionAssignment
            {
                MissionId = missionId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow,
                RoleDescription = "Participant"
            });

            await _context.SaveChangesAsync();
            return AssignResult.Success;
        }

        public async Task<IEnumerable<AssignmentDto>> GetUsersForMissionAsync(Guid missionId)
        {
            var assignments = await _context.MissionAssignments
                .Where(a => a.MissionId == missionId)
                .Include(a => a.User)
                .Select(a => new AssignmentDto
                {
                    UserId    = a.User.Id,
                    Email     = a.User.Email!,
                    AssignedAt = a.AssignedAt
                })
                .ToListAsync();

            return assignments;
        }
    }
}
