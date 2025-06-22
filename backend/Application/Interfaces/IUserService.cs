using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IUserService
{
    Task RegisterVolunteerAsync(RegisterVolunteerRequestDto dto);
    Task RegisterOrganizationAsync(RegisterOrganizationRequestDto dto);
}
