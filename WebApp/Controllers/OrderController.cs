using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using WebAPI.DTOs;
using WebApp.ApiClients;

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

        [HttpPost("AddOrderItems")]
        public async Task<IActionResult> AddOrderItems([FromBody] List<OrderItemDto> items)
        {
            if (items == null || !items.Any())
                return BadRequest("No items provided.");

            foreach (var item in items)
            {
                var res = await _http.PostAsJsonAsync("orderitems", item);
                if (!res.IsSuccessStatusCode)
                    return BadRequest("Failed to add item: " + item.ProductId);
            }

            return Ok("All items added.");
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
