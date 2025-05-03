using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string ReviewerName { get; set; } = "Anonymous User";

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
