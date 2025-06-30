namespace Domain.Entities
{
    public class MissionAssignment
    {
        public int Id { get; set; }

        public Guid MissionId { get; set; }
        public Mission Mission { get; set; } = null!;

        public Guid VolunteerId { get; set; }
        public VolunteerProfile Volunteer { get; set; } = null!;

        public DateTime AssignedAt { get; set; }

        public string? RoleDescription { get; set; }

        public MissionAssignment()
        {
            AssignedAt = DateTime.UtcNow;
        }
    }
}
