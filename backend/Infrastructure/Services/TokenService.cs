using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) =>
            _config = config;

        public string CreateToken(User user)
        {
            // 1) Definiera claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,           user.Email           ?? string.Empty),
                new Claim(ClaimTypes.Role,            user.Role.ToString())
            };

            // 2) Skapa signing key & credentials
            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3) Bygg upp JWT-tokenen
            var token = new JwtSecurityToken(
                issuer:             _config["Jwt:Issuer"],
                audience:           _config["Jwt:Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            // 4) Returnera token som sträng
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
