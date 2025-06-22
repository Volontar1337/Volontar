using Application.DTOs.Auth;
using Application.Interfaces;
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

    public async Task RegisterVolunteerAsync(RegisterVolunteerRequestDto dto)
    {
        // Create User
        var user = new User
        {
            Email = dto.Email,
            Role = UserRole.Volunteer,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        // Link Volunteer Profile
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
    }

    public async Task RegisterOrganizationAsync(RegisterOrganizationRequestDto dto)
    {
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
    }
}
