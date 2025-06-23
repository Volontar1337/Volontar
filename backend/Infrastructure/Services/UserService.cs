using Application.Interfaces;
using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponseDto> RegisterVolunteerAsync(RegisterVolunteerRequestDto dto)
    {
        // Kontrollera om e-postadressen redan finns
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Email = dto.Email,
            Role = UserRole.Volunteer,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        var profile = new VolunteerProfile
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            User = user
        };

        _context.Users.Add(user);
        _context.VolunteerProfiles.Add(profile);

        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Role = user.Role.ToString()
        };
    }

    public async Task<RegisterResponseDto> RegisterOrganizationAsync(RegisterOrganizationRequestDto dto)
    {
        // Kontrollera om e-postadressen redan finns
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Email = dto.Email,
            Role = UserRole.Organization,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        var profile = new OrganizationProfile
        {
            OrganizationName = dto.OrganizationName,
            ContactPerson = dto.ContactPerson,
            PhoneNumber = dto.PhoneNumber,
            Website = dto.Website,
            User = user
        };

        _context.Users.Add(user);
        _context.OrganizationProfiles.Add(profile);

        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Role = user.Role.ToString()
        };
    }
}
