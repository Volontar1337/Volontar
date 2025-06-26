using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterVolunteerRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;

        [Required]
        public string FirstName { get; set; } = default!;

        [Required]
        public string LastName { get; set; } = default!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;
    }
}