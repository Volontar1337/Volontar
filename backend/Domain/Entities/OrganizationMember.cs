namespace Domain.Entities
{
    public class OrganizationMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid OrganizationProfileId { get; set; }
        public OrganizationProfile OrganizationProfile { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public string Role { get; set; } = "Member"; // t.ex. Member, Admin, etc.
    }
}
