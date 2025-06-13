using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WPF.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "api/inventory";

        public InventoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_apiKey}/get_all");

            return await response.Content.ReadFromJsonAsync<IEnumerable<InventoryDto>>();
        }

        public async Task<bool> AddProductToInventoryAsync(string token, InventoryDto inventoryDto)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync($"{_apiKey}/add", inventoryDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInventoryAsync(string token, int inventoryId)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"{_apiKey}/delete/{inventoryId}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateInventoryAsync(string token, InventoryDto inventoryDto)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"{_apiKey}/update/{inventoryDto.Id}", inventoryDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<StockCheckDto>> GetStockCheckHistoryAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_apiKey}/stock_check/history");

            return await response.Content.ReadFromJsonAsync<IEnumerable<StockCheckDto>>();
        }

        public async Task<bool> PerformStockCheckAsync(string token, List<StockCheckDto> stockChecks)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync($"{_apiKey}/stock_check/perform", stockChecks);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ClearStockCheckHistoryAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"{_apiKey}/stock_check/clear");

            return response.IsSuccessStatusCode;
        }
    }
}
