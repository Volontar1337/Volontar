using Domain.Enums;

namespace Domain.Entities;

public class OrganizationProfile
{
    public Guid Id { get; set; } // Primary key
    public Guid UserId { get; set; } // Foreign key to User
    public User User { get; set; } = null!;

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
    
}