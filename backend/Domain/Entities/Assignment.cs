using System;

namespace Domain.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string? Location { get; set; }
        public DateTime Time { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
