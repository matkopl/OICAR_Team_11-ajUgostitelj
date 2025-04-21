namespace WebAPI.DTOs
{
    public class NotificationQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt"; // "CreatedAt", "Message", "UserId"
        public bool SortDescending { get; set; } = true;
        public int? UserId { get; set; } // Opcionalni filter po korisniku
    }
}
