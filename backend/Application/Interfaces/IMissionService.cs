using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId, MissionStatus? status = null);
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId);
    }
}