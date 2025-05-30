using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string Name { get; set; }

        public int UserCount { get; set; }

    }
}
