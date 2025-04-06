using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Invalid role. Allowed values are 1 (Admin) or 2 (User).")]
        public int RoleId { get; set; }
    }
}
