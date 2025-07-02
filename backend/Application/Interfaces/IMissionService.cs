using Application.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetMissionsForUserAsync(Guid userId, MissionStatus? status = null);
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId);
        Task<AssignResult> AssignUserToMissionAsync(Guid missionId, Guid userId);
        Task<List<UserDto>> GetUsersForMissionAsync(Guid missionId);
    }
}
