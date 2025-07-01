using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Application.Services
{
    public class MissionService : IMissionService
    {
        private readonly AppDbContext _context;

        public MissionService(AppDbContext context)
        {
            _context = context;
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

        public async Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId)
        {
            var mission = new Mission
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedByOrgId = organizationId
            };

            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            return mission.Id;
        }

        public async Task<bool> AssignVolunteerToMissionAsync(Guid missionId, Guid volunteerId)
        {
            var alreadyAssigned = await _context.MissionAssignments
                .AnyAsync(ma => ma.MissionId == missionId && ma.VolunteerId == volunteerId);

            if (alreadyAssigned)
            {
                Console.WriteLine($"❌ Volunteer {volunteerId} is already assigned to mission {missionId}");
                return false;
            }

            var assignment = new MissionAssignment
            {
                MissionId = missionId,
                VolunteerId = volunteerId,
                AssignedAt = DateTime.UtcNow
            };

            _context.MissionAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Volunteer {volunteerId} assigned to mission {missionId}");

            return true;
        }
    }
}
