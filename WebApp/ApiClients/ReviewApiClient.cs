using System.Net;
using WebApp.DTOs;

namespace WebApp.ApiClients
{
    public class ReviewApiClient
    {
        private readonly HttpClient _http;

        public ReviewApiClient(HttpClient http)
        {
            _http = http;
        }

        public Task<List<ReviewDTO>> LoadReviewsAsync() =>
            _http.GetFromJsonAsync<List<ReviewDTO>>("review");

        public async Task<ReviewDTO?> LoadReviewAsync(int id)
        {
            var resp = await _http.GetAsync($"review/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ReviewDTO>();
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewDTO newReview)
        {
            var resp = await _http.PostAsJsonAsync("review", newReview);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ReviewDTO>();
        }

        public async Task<bool> UpdateReviewAsync(ReviewDTO updatedReview)
        {
            var resp = await _http.PutAsJsonAsync($"review/{updatedReview.Id}", updatedReview);
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var resp = await _http.DeleteAsync($"review/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<List<ReviewDTO>>LoadReviewsByProductId(int productId)
        {
            return await _http.GetFromJsonAsync<List<ReviewDTO>>($"review/by-product/{productId}");
        }
    }
}
