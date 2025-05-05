using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "http://localhost:5207/api/payment";

        public PaymentRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
       
        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_apiKey}/get_all");

            return await response.Content.ReadFromJsonAsync<IEnumerable<PaymentDto>>();
        }
    }
}
