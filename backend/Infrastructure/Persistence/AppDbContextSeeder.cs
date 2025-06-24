using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public static class AppDbContextSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Database already seeded.");
                return;
            }

            var userId = Guid.Parse("0b185b41-cd69-4294-836f-863310deab86");
            var orgId = Guid.Parse("235cad2a-f261-4854-8eff-220e1a6bb04b");
            var missionId = Guid.Parse("ba98a7a6-90b1-4b1f-9283-9ca470642e99");

            var user = new User
            {
                Id = userId,
                Email = "orguser@test.com",
                PasswordHash = "dummyhash",
                Role = UserRole.Organization
            };

            var orgProfile = new OrganizationProfile
            {
                Id = orgId,
                UserId = userId,
                OrganizationName = "TestOrg",
                ContactPerson = "Testperson",
                PhoneNumber = "0701234567",
                Website = "https://testorg.se",
                CreatedAt = DateTime.UtcNow
            };

            var mission = new Mission
            {
                Id = missionId,
                Title = "Testuppdrag",
                Description = "Beskrivning av testuppdrag",
                Location = "Testgatan 1, Teststad",
                StartTime = DateTime.Parse("2025-07-01T10:00:00Z"),
                EndTime = DateTime.Parse("2025-07-01T14:00:00Z"),
                Status = MissionStatus.Upcoming,
                CreatedByOrgId = orgId
            };

            context.Users.Add(user);
            context.OrganizationProfiles.Add(orgProfile);
            context.Missions.Add(mission);

            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully.");
        }
    }
}
