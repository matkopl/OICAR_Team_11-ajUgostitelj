namespace WebAPI.DTOs
{
    public class OrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "Status"; // "Status", "TotalAmount", "TableId"
        public bool SortDescending { get; set; } = false;
    }
}
