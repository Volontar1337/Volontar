using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public static class AppDbContextSeeder
    {
        private static readonly Guid MockOrgUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid MockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            var userExists = await context.Users.AnyAsync();
            var missionExists = await context.Missions.AnyAsync();

            // 🏢 Seed mock user + organization profile
            if (!userExists)
            {
                var mockOrgUser = new User
                {
                    Id = MockOrgUserId,
                    Email = "mock_orguser@test.com",
                    FirstName = "Mock",
                    LastName = "Organization",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                mockOrgUser.PasswordHash = hasher.HashPassword(mockOrgUser, "supersecret123");

                var orgProfile = new OrganizationProfile
                {
                    Id = MockOrgUserId,
                    UserId = MockOrgUserId,
                    OrganizationName = "Mock Organization",
                    ContactPerson = "Mock Contact",
                    PhoneNumber = "0700000000",
                    Website = "https://mock.org",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(mockOrgUser);
                context.OrganizationProfiles.Add(orgProfile);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded mock organization user and profile.");
            }

            // 🧭 Seed 3 missions
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
                    CreatedByUserId = MockOrgUserId,
                    CreatedByOrgId = MockOrgUserId
                },
                new Mission
                {
                    Id = Guid.NewGuid(),
                    Title = "Active Mission",
                    Description = "This mission is happening now.",
                    Location = "Main Square",
                    StartTime = DateTime.UtcNow.AddHours(-1),
                    EndTime = DateTime.UtcNow.AddHours(2),
                    CreatedByUserId = MockOrgUserId,
                    CreatedByOrgId = MockOrgUserId
                },
                new Mission
                {
                    Id = Guid.NewGuid(),
                    Title = "Upcoming Mission",
                    Description = "This mission will happen in the future.",
                    Location = "New District",
                    StartTime = DateTime.UtcNow.AddDays(3),
                    EndTime = DateTime.UtcNow.AddDays(5),
                    CreatedByUserId = MockOrgUserId,
                    CreatedByOrgId = MockOrgUserId
                }
            };

                context.Missions.AddRange(missions);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded mock missions.");
            }

            // 👤 Seed second user and assign to mission
            var userExistsById = await context.Users.AnyAsync(u => u.Id == MockUserId);
            var assignmentExists = await context.MissionAssignments.AnyAsync();

            if (!userExistsById && !assignmentExists)
            {
                var mockUser = new User
                {
                    Id = MockUserId,
                    Email = "mock_user@test.com",
                    FirstName = "Mock",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                mockUser.PasswordHash = hasher.HashPassword(mockUser, "mockuserpass");

                context.Users.Add(mockUser);
                await context.SaveChangesAsync();

                var activeMission = await context.Missions.FirstOrDefaultAsync(m => m.Title == "Active Mission");
                if (activeMission != null)
                {
                    var assignment = new MissionAssignment
                    {
                        MissionId = activeMission.Id,
                        UserId = mockUser.Id,
                        AssignedAt = DateTime.UtcNow,
                        RoleDescription = "Support staff"
                    };

                    context.MissionAssignments.Add(assignment);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Seeded mock user and mission assignment.");
                }
            }

            logger.LogInformation("Database seeding complete.");
        }
    }
}
