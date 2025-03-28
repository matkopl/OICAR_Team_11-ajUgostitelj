﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int TableId { get; set; } 

        public Table Table { get; set; } 

        public ICollection<OrderItem> OrderItems { get; set; }

        public Payment Payment { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
