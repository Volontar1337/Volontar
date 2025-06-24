using Domain.Enums;

namespace Domain.Entities
{
    public class Mission
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public MissionStatus Status { get; set; } = MissionStatus.Upcoming;

        public Guid CreatedByOrgId { get; set; }
        public OrganizationProfile? CreatedByOrg { get; set; }
    }
}
