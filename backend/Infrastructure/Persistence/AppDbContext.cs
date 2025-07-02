using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
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
        
        // Many-to-one: MissionAssignment -> User
        modelBuilder.Entity<MissionAssignment>()
        .HasOne(ma => ma.User)
        .WithMany()
        .HasForeignKey(ma => ma.UserId);
    }
}
