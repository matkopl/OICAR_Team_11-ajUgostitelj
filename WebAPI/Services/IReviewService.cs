using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<ReviewDTO> CreateReviewAsync(ReviewDTO reviewDTO);
        Task<ReviewDTO> UpdateReviewAsync(int id, ReviewDTO reviewDTO);
        Task<double?> GetAverageRatingAsync(int productId);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId);
    }
}
