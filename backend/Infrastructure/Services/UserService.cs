using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public UserService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            var result = _passwordHasher
                .VerifyHashedPassword(user, user.PasswordHash, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<RegisterResponseDto> RegisterUserAsync(RegisterUserRequestDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = _passwordHasher.HashPassword(new User(), dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.CreateToken(user);

            return new RegisterResponseDto
            {
                UserId = user.Id,
                Token = token
            };
        }

        public async Task<RegisterResponseDto> RegisterOrganizationAsync(RegisterOrganizationRequestDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Email is already registered.");

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

            var token = _tokenService.CreateToken(user);

            return new RegisterResponseDto
            {
                UserId = user.Id,
                Token = token
            };
        }
    }
}
