using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int TableId { get; set; }

        public Table? Table { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Payment? Payment { get; set; }
        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}
