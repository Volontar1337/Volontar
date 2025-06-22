namespace Domain.Entities;

public class VolunteerProfile
{
    public Guid Id { get; set; } // Primary key
    public Guid UserId { get; set; } // Foreign key to User
    public User User { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
