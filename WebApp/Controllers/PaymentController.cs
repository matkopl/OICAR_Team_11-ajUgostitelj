using Microsoft.AspNetCore.Mvc;
using WebApp.ApiClients;
using WebApp.ViewModels;
using WebAPI.DTOs;
using WebAPI.Models;
using System.Text.Json;
using WebAPI;
using WebApp.Models;
using static System.Net.WebRequestMethods;
using System.Net.Http;

namespace WebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PaymentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet, HttpPost]
        public IActionResult Checkout()
        {
            var cart = GetCartFromTempData();
            TempData.Keep("Cart");
            return View(cart);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Process([FromForm] string method)
        {
            var cart = TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!)!
                : new List<ProductCartViewModel>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://oicar-team-11-ajugostitelj-11.onrender.com/api/");

            var orderDto = new OrderDto
            {
                OrderDate = DateTime.UtcNow,
                TableId = 2,
                Status = "Paid",
                TotalAmount = cart.Sum(x => x.Price * x.Quantity)
            };

            // narudzba
            var orderResponse = await client.PostAsJsonAsync("order", orderDto);
            if (!orderResponse.IsSuccessStatusCode)
                return View("Checkout");

            var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();

            // kreiranje itemsa narudzbe
            var itemDtos = cart.Select(c => new OrderItemDto
            {
                OrderId = createdOrder!.Id,
                ProductId = c.Id,
                ProductName = c.Name,
                UnitPrice = c.Price,
                Quantity = c.Quantity
            }).ToList();

            foreach (var item in itemDtos)
            {
                var res = await client.PostAsJsonAsync("orderItems", item);
                if (!res.IsSuccessStatusCode)
                    return BadRequest("Failed to add item: " + item.ProductId);
            }
            
            var paymentDto = new PaymentDto
            {
                Amount = createdOrder!.TotalAmount,
                Method = "CreditCard",
                PaymentDate = DateTime.UtcNow,
                OrderId = createdOrder.Id
            };
            //slanje paymenta u bazu
            var paymentResponse = await client.PostAsJsonAsync("payment/create", paymentDto);
            if (!paymentResponse.IsSuccessStatusCode)
                return View("Checkout");

            // ovo nam treba za cuvanje ID za tracking, ne brisati
            HttpContext.Session.SetInt32("LastOrderId", createdOrder.Id);

            return RedirectToAction("Success", new { id = createdOrder.Id, orderStatus = createdOrder.Status, table = orderDto.TableId });
        }

        public IActionResult Success(int id, string orderStatus, int table)
        {
            ViewBag.OrderId = id;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.TableNumber = table;
            return View("Success");
        }
        private List<ProductCartViewModel> GetCartFromTempData()
        {
            return TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!) ?? new()
                : new();
        }
    }
}
