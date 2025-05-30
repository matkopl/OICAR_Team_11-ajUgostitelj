using System.Net;
using WebApp.DTOs;

namespace WebApp.ApiClients
{
    public class CategoriesApiClient
    {
        private readonly HttpClient _http;

        public CategoriesApiClient(HttpClient http)
        {
            _http = http;
        }

        public Task<List<CategoryDto>> LoadCategoriesAsync() =>
            _http.GetFromJsonAsync<List<CategoryDto>>("categories");

        public async Task<CategoryDto?> LoadCategoryAsync(int id)
        {
            var resp = await _http.GetAsync($"categories/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto newCategory)
        {
            var resp = await _http.PostAsJsonAsync("categories", newCategory);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto updatedCategory)
        {
            var resp = await _http.PutAsJsonAsync($"categories/{updatedCategory.Id}", updatedCategory);
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var resp = await _http.DeleteAsync($"categories/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }
    }
}
