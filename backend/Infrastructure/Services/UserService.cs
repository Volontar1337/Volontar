using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
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

    // Login
    // This method authenticates a user by checking their email and password.
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        // Look up user by email
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return null;

        // Verify hashed password
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Success)
            return user;

        // Invalid login
        return null;
    }

    // TODO: Denna metod tas bort i steg 4 när vi förenklar registreringen
    public async Task<RegisterResponseDto> RegisterVolunteerAsync(RegisterVolunteerRequestDto dto)
    {
        // OBS: Denna metod ersätts i steg 4
        var user = new User
        {
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = _passwordHasher.HashPassword(new User(), dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id
        };
    }


    public async Task<RegisterResponseDto> RegisterOrganizationAsync(RegisterOrganizationRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Email = dto.Email,
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
            UserId = user.Id
        };
    }

}
