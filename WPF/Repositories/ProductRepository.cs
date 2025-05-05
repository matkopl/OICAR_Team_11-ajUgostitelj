using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private const string BaseUrl = "api/product";
        private readonly HttpClient _httpClient;

        public ProductRepository(HttpClient httpClient) => _httpClient = httpClient;

        private void SetAuth(string token) =>
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        public async Task<IEnumerable<ProductDto>> GetAllAsync(string token)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync(BaseUrl);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        }

        public async Task<ProductDto> CreateAsync(string token, ProductDto product)
        {
            SetAuth(token);
            var resp = await _httpClient.PostAsJsonAsync(BaseUrl, product);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task UpdateAsync(string token, int id, ProductDto product)
        {
            SetAuth(token);
            var resp = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", product);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string token, int id)
        {
            SetAuth(token);
            var resp = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
