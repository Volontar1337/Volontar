public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Role { get; set; } = default!;
}
