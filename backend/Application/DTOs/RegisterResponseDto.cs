namespace Application.DTOs
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = default!;
    }
}