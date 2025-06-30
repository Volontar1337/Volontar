namespace Application.DTOs
{
    /// <summary>
    /// DTO for creating a mission assignment (volunteer signs up for a mission).
    /// </summary>
    public class MissionAssignmentCreateDto
    {
        public Guid MissionId { get; set; }             // Mission to join
        public Guid VolunteerId { get; set; }           // The volunteer joining
        public string? RoleDescription { get; set; }    // Optional description (e.g. "Help desk", "Photographer")
    }
}
