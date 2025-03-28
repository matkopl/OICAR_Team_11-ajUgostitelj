using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Method { get; set; } 

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public int OrderId { get; set; } 

        public Order Order { get; set; } 
    }
}
