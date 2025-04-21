using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Order ID")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Product ID")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Dodatne informacije
        public string? ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice => Quantity * UnitPrice;
    }
}
