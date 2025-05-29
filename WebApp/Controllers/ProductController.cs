using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAPI.DTOs;
using WebApp.ApiClients;
using WebApp.ViewModels;
using WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Diagnostics;



namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ProductApiClient _productApiClient;
        private readonly CategoriesApiClient _categoriesApiClient;

        public ProductController(ProductApiClient productApiClient, CategoriesApiClient categoriesApiClient, AppDbContext context)
        {
            _productApiClient = productApiClient;
            _categoriesApiClient = categoriesApiClient;
            _context = context;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var products = await _productApiClient.LoadProductsAsync();
            var categories = await _categoriesApiClient.LoadCategoriesAsync();

            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products.Where(p => categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name == category).ToList();
            }

            var vm = new ProductIndexViewModel
            {
                Products = products.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "Nema kategoriju",
                    AverageRating = p.AverageRating
                }).ToList(),
                Categories = categories.Select(c => c.Name).Distinct().ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productApiClient.LoadProductAsync(id);
            var category = product != null ? await _categoriesApiClient.LoadCategoryAsync(product.CategoryId) : null;

            if (product == null)
                return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = category?.Name ?? "Nema kategoriju",
                AverageRating = product.AverageRating
            };

            return View(vm);
        }

        [HttpPost("by-ids")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("No product IDs provided.");

            var allProducts = await _productApiClient.LoadProductsAsync();
            var filteredProducts = allProducts.Where(p => ids.Contains(p.Id)).ToList();

            return Ok(filteredProducts);
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
        [HttpGet]
        public async Task<IActionResult> ProductReviews()
        {
            var products = await _context.Products.ToListAsync();
            var reviews = await _context.Reviews.Include(r => r.Product).OrderByDescending(r => r.ReviewDate).ToListAsync();

            var vm = new ProductReviewsViewModel
            {
                Products = products,
                Reviews = reviews
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ProductReviews(ProductReviewsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Products = await _context.Products.ToListAsync();
                model.Reviews = await _context.Reviews.Include(r => r.Product).OrderByDescending(r => r.ReviewDate).ToListAsync();
                return View(model);
            }

            var selectedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.NewReview.ProductId);
            if (selectedProduct == null)
            {
                ModelState.AddModelError("", "Selected product not found");
                return View(model);
            }

            model.NewReview.ProductName = selectedProduct.Name;
            model.NewReview.ReviewDate = DateTime.UtcNow;

            _context.Reviews.Add(model.NewReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductReviews");
        }
    }
}

