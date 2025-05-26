using System.Security.AccessControl;

namespace WebAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public string ReviewerName { get; set; } = "Testni user!";

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }

    }
}
