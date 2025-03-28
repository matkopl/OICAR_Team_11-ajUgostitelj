using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WebAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PwdHash { get; set; }

        [Required]
        public string PwdSalt { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
