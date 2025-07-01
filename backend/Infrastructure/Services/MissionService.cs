using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<AssignResult> AssignVolunteerToMissionAsync(Guid missionId, Guid volunteerId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null)
                return AssignResult.MissionNotFound;

            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            if (volunteer == null)
                return AssignResult.VolunteerNotFound;

            var alreadyAssigned = await _context.MissionAssignments
                .AnyAsync(ma => ma.MissionId == missionId && ma.VolunteerId == volunteerId);

            if (alreadyAssigned)
                return AssignResult.AlreadyAssigned;

            _context.MissionAssignments.Add(new MissionAssignment
            {
                MissionId = missionId,
                VolunteerId = volunteerId,
                AssignedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return AssignResult.Success;
        }

        public async Task<List<VolunteerDto>> GetVolunteersForMissionAsync(Guid missionId)
        {
            return await _context.MissionAssignments
                .Where(ma => ma.MissionId == missionId)
                .Include(ma => ma.Volunteer)
                .ThenInclude(v => v.User)  // För att få volontärens email via User
                .Select(ma => new VolunteerDto
                {
                    Id = ma.Volunteer.Id,
                    FirstName = ma.Volunteer.FirstName,
                    LastName = ma.Volunteer.LastName,
                    Email = ma.Volunteer.User.Email
                })
                .ToListAsync();
        }
    }
}
