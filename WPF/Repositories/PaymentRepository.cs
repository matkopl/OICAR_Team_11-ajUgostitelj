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
        private readonly string _apiKey = "api/payment";

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

        public async Task<bool> UpdatePaymentAsync(string token, PaymentDto payment)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"{_apiKey}/update/{payment.Id}", payment);

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating payment: {e.Message}");
            }
        }

        public async Task<bool> DeletePaymentAsync(string token, int paymentId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"{_apiKey}/delete/{paymentId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting payment: {e.Message}");
            }
        }
    }
}
