namespace Application.DTOs
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = null!;
    }
}
