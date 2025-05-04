namespace WebApp.Models
{
    public class CreateOrderRequest
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public int TableId { get; set; } = 1;
        public List<OrderItemRequest> OrderItems { get; set; }

    }
}
