namespace Application.DTOs
{
    public class MissionAssignmentCreateDto
    {
        public Guid MissionId { get; set; }             // Mission to join
        public string? RoleDescription { get; set; }    // Optional description (e.g. "Help desk", "Photographer")
    }
}
