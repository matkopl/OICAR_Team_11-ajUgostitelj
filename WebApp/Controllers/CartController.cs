using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.ViewModels;
using WebApp.Extensions;
using WebAPI.Models;
using WebAPI;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {

        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!)!
                : new List<ProductCartViewModel>();

            TempData.Keep("Cart");
            return View(cart);
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!)!
                : new List<ProductCartViewModel>();

            cart = cart.Where(x => x.Id != id).ToList();

            TempData["Cart"] = JsonSerializer.Serialize(cart);
            TempData.Keep("Cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutFromClient([FromBody] List<ProductCartViewModel> items)
        {
            if (items == null || !items.Any())
                return BadRequest("Cart is empty.");

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                TableId = 2, // hardcoded for now
                Status = OrderStatus.Pending,
                TotalAmount = items.Sum(x => x.Price * x.Quantity)
            };

            foreach (var item in items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.Id,
                    Quantity = item.Quantity
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, orderId = order.Id });
        }


        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }


    }
}
