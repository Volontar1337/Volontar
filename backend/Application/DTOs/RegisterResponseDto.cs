namespace Application.DTOs
{
    public class RegisterResponseDto
    {
        public Guid   UserId { get; set; }
        // Samma Token-egenskap som LoginResponseDto
        public string Token  { get; set; } = default!;
    }
}