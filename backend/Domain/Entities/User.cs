using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public OrganizationProfile? OrganizationProfile { get; set; }
    public ICollection<OrganizationMember> OrganizationMemberships { get; set; } = new List<OrganizationMember>();

}