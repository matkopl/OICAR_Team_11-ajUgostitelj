using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
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
        public string Password { get; set; }
    }
}
