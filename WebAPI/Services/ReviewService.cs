using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class ReviewService : IReviewService
    {

        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public ReviewService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var reviews = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewDTO reviewDTO)
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var review = _mapper.Map<Review>(reviewDTO);
            review.ReviewDate = DateTime.UtcNow;

            await repo.AddAsync(review);
            await repo.SaveChangesAsync();

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var review = await repo.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            repo.Remove(review);
            await repo.SaveChangesAsync();
        }


        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var review = await repo.GetByIdAsync(id);
            return review != null ? _mapper.Map<ReviewDTO>(review) : null;
        }

        public async Task<ReviewDTO> UpdateReviewAsync(int id, ReviewDTO reviewDTO)
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var existing = await repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Review not found");

            _mapper.Map(reviewDTO, existing);
            repo.Update(existing);
            await repo.SaveChangesAsync();

            return _mapper.Map<ReviewDTO>(existing);
        }

        public async Task<double?> GetAverageRatingAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId)
        {
            var repo = _repositoryFactory.GetRepository<Review>();
            var reviews = await repo.GetAllAsync();
            var filtered = reviews.Where(r => r.ProductId == productId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(filtered);
        }

    }
}
