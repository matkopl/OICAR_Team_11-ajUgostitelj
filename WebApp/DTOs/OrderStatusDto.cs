namespace WebAPI.DTOs
{
    public class OrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } 
        public string? Notes { get; set; } 
    }
}
