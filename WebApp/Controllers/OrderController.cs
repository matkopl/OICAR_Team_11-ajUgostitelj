using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using WebAPI.DTOs;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly HttpClient _http;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://oicar-team-11-ajugostitelj-11.onrender.com/api/");
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderDto order)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("order", order);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    return BadRequest($"API Error: {errorText}");
                }

                var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
