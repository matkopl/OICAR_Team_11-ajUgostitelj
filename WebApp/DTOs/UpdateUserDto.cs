using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(8)]
        public string? Password { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Invalid role. Allowed values are 1 (Admin) or 2 (User).")]
        public int RoleId { get; set; }
    }
}
