using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetAllAsync();
        Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId, MissionStatus? status = null);
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId);
        Task<AssignResult> AssignVolunteerToMissionAsync(Guid missionId, Guid volunteerId);
        Task<List<VolunteerDto>> GetVolunteersForMissionAsync(Guid missionId);
    }
}