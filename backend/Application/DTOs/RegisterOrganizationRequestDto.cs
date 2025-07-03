using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterOrganizationRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;

        [Required]
        public string OrganizationName { get; set; } = default!;

        [Required]
        public string ContactPerson { get; set; } = default!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;

        [Url]
        public string? Website { get; set; }
    }
}