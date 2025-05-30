namespace WebApp.DTOs
{
    public class TableQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool? IsOccupied { get; set; }
    }
}
