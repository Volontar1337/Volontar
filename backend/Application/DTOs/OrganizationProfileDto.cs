namespace Application.DTOs
{
    public class OrganizationProfileDto
    {
        public string OrganizationName { get; set; } = default!;
        public string ContactPerson { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? Website { get; set; }
    }
}