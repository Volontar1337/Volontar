using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IOrganizationProfileService
{
    Task<IEnumerable<OrganizationProfile>> GetAllAsync();
    Task<OrganizationProfile?> GetByIdAsync(Guid id);
    Task<(bool Success, string? ErrorMessage, OrganizationProfile? Profile)> CreateAsync(Guid userId, OrganizationProfileDto dto);
}
