using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<User> Users { get; } = new List<User>();
    }
}
