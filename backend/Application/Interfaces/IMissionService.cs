using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId);
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId);
    }
}