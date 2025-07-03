namespace Application.DTOs
{
    public class LoginResponseDto
    {
        // JWT-token som klienten skall använda i sina API-anrop
        public string Token { get; set; } = default!;
    }
}