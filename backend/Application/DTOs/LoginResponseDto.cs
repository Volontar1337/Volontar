namespace Application.DTOs
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        
        // ✅ Ny property för organisationer som användaren är admin för
        public List<SimpleOrganizationDto> CreatedOrganizations { get; set; } = new();
    }
}