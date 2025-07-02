namespace Application.DTOs;

public class RegisterUserRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
