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

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"order/{id}");
        }

        public async Task<string?> GetOrderStatusAsync(int? orderId)
        {
            var response = await _httpClient.GetAsync($"order/{orderId}/status");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<PaymentDto?> CreatePaymentAsync(PaymentDto payment)
        {
            var response = await _httpClient.PostAsJsonAsync("payment/create", payment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentDto>();
        }
    }
}
