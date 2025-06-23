using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IUserService
{
    Task<RegisterResponseDto> RegisterVolunteerAsync(RegisterVolunteerRequestDto dto);
    Task<RegisterResponseDto> RegisterOrganizationAsync(RegisterOrganizationRequestDto dto);
}
