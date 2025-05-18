using System.ComponentModel.DataAnnotations;
using WebAPI.Models;

namespace WebAPI.DTOs
{
    public class StockCheckDto
    {
        public int Id { get; set; }

        [Required]
        public DateTime CheckDate { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public int RecordedQuantity { get; set; }

        [Required]
        public int ActualQuantity { get; set; }

        public int Difference => ActualQuantity - RecordedQuantity;
    }
}
