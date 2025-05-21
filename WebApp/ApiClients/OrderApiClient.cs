using WebAPI.DTOs;
using static System.Net.WebRequestMethods;

namespace WebApp.ApiClients
{
    public class OrderApiClient
    {
        private readonly HttpClient _httpClient;

        public OrderApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderDto?> GetOrderById(int id)
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"order/{id}");
        }

        public async Task<string?> GetOrderStatus(int? orderId)
        {
            var response = await _httpClient.GetAsync($"order/{orderId}/status");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<OrderDto?> CreateOrder(OrderDto newOrder)
        {
            var response = await _httpClient.PostAsJsonAsync("order", newOrder);
            response.EnsureSuccessStatusCode();
            var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
            return createdOrder;
        }
        
        public async Task<bool> AddOrderItemsToOrder(List<OrderItemDto> newOrderItems)
        {
            foreach (var item in newOrderItems) {
                var added = await _httpClient.PostAsJsonAsync("orderItems", item);
                if (!added.IsSuccessStatusCode) return false;
            
            }
            return true;
        }

    }
}
