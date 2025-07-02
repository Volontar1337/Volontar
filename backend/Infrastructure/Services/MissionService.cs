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



        public async Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid userId)
        {
            // Hämta användaren (ska alltid finnas om userId kommer från claims)
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            // Hämta OrganizationProfile för användaren om det finns
            var orgProfile = await _context.OrganizationProfiles
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            var mission = new Mission
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedByUserId = userId,
                CreatedByUser = user,
                CreatedByOrgId = orgProfile?.Id,      
                CreatedByOrg = orgProfile             
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
                RoleDescription = "Participant" // eller annan passande rollbeskrivning
            });

            await _context.SaveChangesAsync();
            return AssignResult.Success;
        }

        public async Task<List<UserDto>> GetUsersForMissionAsync(Guid missionId)
        {
            return await _context.MissionAssignments
                .Where(ma => ma.MissionId == missionId)
                .Include(ma => ma.User)
                .Select(ma => new UserDto
                {
                    Id = ma.User.Id,
                    FirstName = ma.User.FirstName,
                    LastName = ma.User.LastName,
                    Email = ma.User.Email
                })
                .ToListAsync();
        }
    }
}
