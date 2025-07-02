using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<VolunteerProfile> VolunteerProfiles => Set<VolunteerProfile>();
    public DbSet<VolunteerProfile> Volunteers { get; set; }
    public DbSet<OrganizationProfile> OrganizationProfiles => Set<OrganizationProfile>();
    public DbSet<MissionAssignment> MissionAssignments { get; set; }
    public DbSet<Mission> Missions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One-to-one: User -> OrganizationProfile
        modelBuilder.Entity<User>()
            .HasOne(u => u.OrganizationProfile)
            .WithOne(o => o.User)
            .HasForeignKey<OrganizationProfile>(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-one: Mission -> CreatedByUser
        modelBuilder.Entity<Mission>()
            .HasOne(m => m.CreatedByUser)
            .WithMany()
            .HasForeignKey(m => m.CreatedByUserId);
    }
}
