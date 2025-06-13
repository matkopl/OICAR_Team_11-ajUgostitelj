using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private const string BaseUrl = "api/categories";
        private readonly HttpClient _httpClient;

        public CategoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetAuth(string token) =>
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(string token)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync(BaseUrl);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        }

        public async Task<CategoryDto> CreateAsync(string token, CategoryDto category)
        {
            SetAuth(token);
            var resp = await _httpClient.PostAsJsonAsync(BaseUrl, category);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task UpdateAsync(string token, CategoryDto category)
        {
            SetAuth(token);
            var resp = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{category.Id}", category);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string token, int categoryId)
        {
            SetAuth(token);
            var resp = await _httpClient.DeleteAsync($"{BaseUrl}/{categoryId}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
