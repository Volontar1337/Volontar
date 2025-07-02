namespace Application.DTOs
{
    public class OrganizationProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string OrganizationName { get; set; } = default!;
        public string ContactPerson { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? Website { get; set; }

        public UserDto? User { get; set; }
    }
}