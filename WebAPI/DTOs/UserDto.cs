using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        public bool IsAnonymized { get; set; }
    }
}
