using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class TableDto
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Table name is required")]
        [MaxLength(20, ErrorMessage = "Table name cannot exceed 20 characters")]
        public string Name { get; set; }

        public bool IsOccupied { get; set; } = false; 
    }
}
