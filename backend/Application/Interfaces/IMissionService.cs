using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetMissionsForUserAsync(Guid userId, MissionStatus? statusFilter = null);
        
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid userId);
        
        Task<AssignResult> AssignUserToMissionAsync(Guid missionId, Guid userId);
        
        Task<IEnumerable<AssignmentDto>> GetUsersForMissionAsync(Guid missionId);
    }
}