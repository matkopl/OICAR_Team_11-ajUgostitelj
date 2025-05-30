using WebApp.DTOs;

namespace WebApp.ApiClients
{
    public class PaymentApiClient
    {
        private readonly HttpClient _httpClient;

        public PaymentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PaymentDto?>> GetAllPayments()
        {
            return await _httpClient.GetFromJsonAsync<List<PaymentDto>>("payment/get_all");
        }

        public async Task<PaymentDto?> CreatePaymentAsync(PaymentDto payment)
        {
            var response = await _httpClient.PostAsJsonAsync("payment/create", payment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentDto>();
        }
    }
}
