using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; } 

        public Order Order { get; set; } 

        [Required]
        public int ProductId { get; set; } 

        public Product Product { get; set; } 

        [Required]
        public int Quantity { get; set; }
    }
}
