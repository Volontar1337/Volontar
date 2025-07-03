namespace Application.DTOs
{
    public class AssignmentDto
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}