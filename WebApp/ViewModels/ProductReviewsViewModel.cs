using WebAPI.Models;

namespace WebApp.ViewModels
{
    public class ProductReviewsViewModel
    {
        public List<WebAPI.Models.Product> Products { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public Review NewReview { get; set; } = new();
    }
}
