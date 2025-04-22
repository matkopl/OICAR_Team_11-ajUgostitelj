using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/table";

        public TableRepository(HttpClient httpClient) => _httpClient = httpClient;

        private void SetAuth(string token) =>
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        public async Task<IEnumerable<TableDto>> GetAllAsync(string token)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync(BaseUrl);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<TableDto>>();
        }

        public async Task<TableDto> CreateAsync(string token, TableDto table)
        {
            SetAuth(token);
            var resp = await _httpClient.PostAsJsonAsync(BaseUrl, table);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<TableDto>();
        }

        public async Task DeleteAsync(string token, int id)
        {
            SetAuth(token);
            var resp = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
