using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
