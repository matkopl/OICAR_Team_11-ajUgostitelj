using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } 

        [Required]
        public bool IsOccupied { get; set; }

        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}
