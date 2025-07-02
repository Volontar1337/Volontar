using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public static class AppDbContextSeeder
    {
        private static readonly Guid MockOrgId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid MockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            var userExists = await context.Users.AnyAsync();
            var missionExists = await context.Missions.AnyAsync();

            // 🏢 Seed mock organization + user
            if (!userExists)
            {
                var orgUser = new User
                {
                    Id = MockOrgId,
                    Email = "mock_orguser@test.com",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                orgUser.PasswordHash = hasher.HashPassword(orgUser, "supersecret123");

                var orgProfile = new OrganizationProfile
                {
                    Id = MockOrgId,
                    UserId = MockOrgId,
                    OrganizationName = "Mock Organization",
                    ContactPerson = "Mock Contact",
                    PhoneNumber = "0700000000",
                    Website = "https://mock.org",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(orgUser);
                context.OrganizationProfiles.Add(orgProfile);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded mock organization user.");
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
                        CreatedByUserId = MockOrgId,
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
                        CreatedByUserId = MockOrgId,
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
                        CreatedByUserId = MockOrgId,
                        CreatedByOrgId = MockOrgId
                    }
                };

                context.Missions.AddRange(missions);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded mock missions.");
            }

            // 👤 Seed mock user and mission assignment
            var userExistsById = await context.Users.AnyAsync(u => u.Id == MockUserId);
            var assignmentExists = await context.MissionAssignments.AnyAsync();

            if (!userExistsById && !assignmentExists)
            {
                var volunteerUser = new User
                {
                    Id = MockUserId,
                    Email = "mock_user@test.com",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                volunteerUser.PasswordHash = hasher.HashPassword(volunteerUser, "volunteerpass");

                context.Users.Add(volunteerUser);
                await context.SaveChangesAsync();

                var activeMission = await context.Missions.FirstOrDefaultAsync(m => m.Title == "Active Mission");
                if (activeMission != null)
                {
                    var assignment = new MissionAssignment
                    {
                        MissionId = activeMission.Id,
                        UserId = volunteerUser.Id,
                        AssignedAt = DateTime.UtcNow,
                        RoleDescription = "Food distributor"
                    };

                    context.MissionAssignments.Add(assignment);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Seeded user and mission assignment.");
                }
            }

            logger.LogInformation("Database seeding complete.");
        }
    }
}
