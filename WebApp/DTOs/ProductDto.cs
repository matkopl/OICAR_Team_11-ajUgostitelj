
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        [Url]
        public string ImageUrl { get; set; } = "/images/placeholder.png";
    }
}
