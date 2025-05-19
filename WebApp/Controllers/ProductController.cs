using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAPI.DTOs;
using WebApp.ApiClients;
using WebApp.ViewModels;
using WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category);
            }

            var products = await productsQuery.ToListAsync();

            var categories = await _context.Categories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            var vm = new ProductIndexViewModel
            {
                Products = products.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImgURL,
                    CategoryName = p.Category?.Name ?? "",
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (double?)null
                }).ToList(),
                Categories = categories
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImgURL,
                CategoryName = product.Category?.Name ?? "",
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : (double?)null
            };

            return View(vm);
        }

        [HttpPost("by-ids")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("No product IDs provided.");

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }
        [HttpPost]
        public IActionResult AddToCart(ProductCartViewModel model)
        {
            var cart = TempData.ContainsKey("Cart")
                ? JsonSerializer.Deserialize<List<ProductCartViewModel>>(TempData["Cart"]!.ToString()!)!
                : new List<ProductCartViewModel>();

            var existing = cart.FirstOrDefault(x => x.Id == model.Id);
            if (existing != null)
                existing.Quantity += model.Quantity;
            else
                cart.Add(model);

            TempData["Cart"] = JsonSerializer.Serialize(cart);
            TempData.Keep("Cart");

            return RedirectToAction("Index", "Cart");
        }
        public ActionResult ProductReviews()
        {
            return View();
        }
    }
}

