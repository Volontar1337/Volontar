namespace Application.DTOs
{
    public class OrganizationMemberDto
    {
        public Guid Id { get; set; }
        public UserDto? User { get; set; }
        public string? RoleDescription { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}