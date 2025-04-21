using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WebApp.ApiClients
{
    public class ProductApiClient
    {
        private readonly HttpClient _http;

        public ProductApiClient(HttpClient http)
        {
            _http = http;
        }

        public Task<List<ProductDto>> LoadProductsAsync() =>
            _http.GetFromJsonAsync<List<ProductDto>>("product");

        public async Task<ProductDto?> LoadProductAsync(int id)
        {
            var resp = await _http.GetAsync($"product/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto newProduct)
        {
            var resp = await _http.PostAsJsonAsync("product", newProduct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<bool> UpdateProductAsync(ProductDto updatedProduct)
        {
            var resp = await _http.PutAsJsonAsync($"product/{updatedProduct.Id}", updatedProduct);
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var resp = await _http.DeleteAsync($"product/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }
    }
}
