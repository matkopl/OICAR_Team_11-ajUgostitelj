namespace WebApp.DTOs
{
    public class OrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public OrderStatus? Status { get; set; }
    }
}
