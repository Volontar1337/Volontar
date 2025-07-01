using Application.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<MissionDto>> GetMissionsByOrganizationIdAsync(Guid organizationId, MissionStatus? status = null);
        Task<Guid> CreateMissionAsync(CreateMissionDto dto, Guid organizationId);
        Task<AssignResult> AssignVolunteerToMissionAsync(Guid missionId, Guid volunteerId);
        Task<List<VolunteerDto>> GetVolunteersForMissionAsync(Guid missionId);
    }
}
