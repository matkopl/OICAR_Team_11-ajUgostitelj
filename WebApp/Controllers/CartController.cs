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
            var cart = GetCartFromTempData();
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

            TempData["Cart"] = JsonSerializer.Serialize(items);
            return Json(new { redirectUrl = Url.Action("Checkout", "Payment") });
        }


        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
        private List<ProductCartViewModel> GetCartFromTempData()
        {
            return TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!) ?? new()
                : new();
        }

    }
}