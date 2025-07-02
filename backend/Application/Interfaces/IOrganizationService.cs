using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOrganizationService
    {
        Task<List<OrganizationProfile>> GetAllAsync();

        Task<OrganizationProfile?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid userId, string organizationName);
        Task<OrganizationProfile?> CreateAsync(Guid userId, OrganizationProfileDto dto);
    }
}
