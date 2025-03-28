using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; } 

        public Product Product { get; set; } 

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
