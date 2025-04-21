using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Product ID")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; }

        public string? ProductName { get; set; }
    }
}
