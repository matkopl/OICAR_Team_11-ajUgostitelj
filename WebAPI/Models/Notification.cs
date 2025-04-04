using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int UserId { get; set; } 

        public User? User { get; set; } 
    }
}
