using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password  { get; set; }

    }
}
