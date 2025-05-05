using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
    }
}
