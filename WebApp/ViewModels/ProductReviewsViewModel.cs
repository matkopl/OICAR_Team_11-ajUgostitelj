using WebApp.DTOs;

namespace WebApp.ViewModels
{
    public class ProductReviewsViewModel
    {
        public List<ProductDto> Products { get; set; } = new();
        public List<ReviewDTO> Reviews { get; set; } = new();
        public ReviewDTO NewReview { get; set; } = new();
    }
}
