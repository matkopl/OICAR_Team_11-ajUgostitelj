using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using System.Net.Http.Json;

namespace WPF.Repositories
{
    // OrderRepository.cs
    public class OrderRepository : IOrderRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/order";

        public OrderRepository(HttpClient httpClient) => _httpClient = httpClient;

        private void SetAuth(string token) =>
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        public async Task<IEnumerable<OrderDto>> GetAllAsync(string token)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync(_baseUrl);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
        }

        public async Task<IEnumerable<OrderItemDto>> GetItemsAsync(string token, int orderId)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            
            var resp = await _httpClient.GetAsync($"api/orderitems/order/{orderId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<OrderItemDto>>();
        }

        public async Task<IEnumerable<OrderDto>> GetByTableAsync(string token, int tableId)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync($"{_baseUrl}/byTable/{tableId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
        }

        public async Task<IEnumerable<OrderDto>> GetByPaymentMethodAsync(string token, string paymentMethod)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync($"{_baseUrl}/byPayment/{paymentMethod}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
        }

        public async Task<OrderDto> GetByIdAsync(string token, int orderId)
        {
            SetAuth(token);
            var resp = await _httpClient.GetAsync($"{_baseUrl}/{orderId}/details");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<OrderDto>();
        }

        public async Task<bool> UpdateOrderStatusAsync(string token, OrderStatusDto orderStatusDto)
        {
            SetAuth(token);
            var resp = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{orderStatusDto.OrderId}/status", orderStatusDto);
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to update order {orderStatusDto.OrderId} status. Error: {resp.ReasonPhrase}");
            }

            return resp.IsSuccessStatusCode;
        }
    }

}
