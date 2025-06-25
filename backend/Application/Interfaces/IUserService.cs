using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<RegisterResponseDto> RegisterVolunteerAsync(RegisterVolunteerRequestDto dto);
    Task<RegisterResponseDto> RegisterOrganizationAsync(RegisterOrganizationRequestDto dto);
}
