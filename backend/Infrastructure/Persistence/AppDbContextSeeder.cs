using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public static class AppDbContextSeeder
    {
        private static readonly Guid MockOrgId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            var userExists = await context.Users.AnyAsync();
            var missionExists = await context.Missions.AnyAsync();

            if (!userExists)
            {
                var user = new User
                {
                    Id = MockOrgId,
                    Email = "mock_orguser@test.com",
                    PasswordHash = "not_relevant_for_mock",
                    Role = UserRole.Organization
                };

                var orgProfile = new OrganizationProfile
                {
                    Id = MockOrgId,             // ✅ ID matches user and mission references
                    UserId = MockOrgId,
                    OrganizationName = "Mock Organization",
                    ContactPerson = "Mock Contact",
                    PhoneNumber = "0700000000",
                    Website = "https://mock.org",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, "supersecret123");

                context.Users.Add(user);
                context.OrganizationProfiles.Add(orgProfile);

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded mock organization user.");
            }

            if (!missionExists)
            {
                var missions = new List<Mission>
                {
                    new Mission
                    {
                        Id = Guid.NewGuid(),
                        Title = "Completed Mission",
                        Description = "This mission has already ended.",
                        Location = "Old Town",
                        StartTime = DateTime.UtcNow.AddDays(-10),
                        EndTime = DateTime.UtcNow.AddDays(-5),
                        CreatedByOrgId = MockOrgId
                    },
                    new Mission
                    {
                        Id = Guid.NewGuid(),
                        Title = "Active Mission",
                        Description = "This mission is happening now.",
                        Location = "Main Square",
                        StartTime = DateTime.UtcNow.AddHours(-1),
                        EndTime = DateTime.UtcNow.AddHours(2),
                        CreatedByOrgId = MockOrgId
                    },
                    new Mission
                    {
                        Id = Guid.NewGuid(),
                        Title = "Upcoming Mission",
                        Description = "This mission will happen in the future.",
                        Location = "New District",
                        StartTime = DateTime.UtcNow.AddDays(3),
                        EndTime = DateTime.UtcNow.AddDays(5),
                        CreatedByOrgId = MockOrgId
                    }
                };

                context.Missions.AddRange(missions);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded mock missions.");
            }

            logger.LogInformation("Database seeding complete.");
        }
    }
}
