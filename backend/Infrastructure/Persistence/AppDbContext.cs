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
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
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

        // Ny konfiguration för OrganizationMember
        modelBuilder.Entity<OrganizationMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.OrganizationMemberships)   // Navigationsproperty i User
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.OrganizationProfile)
                .WithMany(o => o.Members)                   // Navigationsproperty i OrganizationProfile
                .HasForeignKey(e => e.OrganizationProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);
        });
    }
}
