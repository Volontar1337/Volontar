namespace Application.DTOs
{
    public class MissionAssignmentCreateDto
    {
        public string? Location { get; set; }

        /// <summary>
        /// Time of the assignment (in local time).
        /// The server will convert this to UTC automatically using ToUniversalTime().
        /// Example input: "2025-07-01T10:00:00"
        /// </summary>
        public DateTime Time { get; set; }

        public string? Description { get; set; }
    }
}
