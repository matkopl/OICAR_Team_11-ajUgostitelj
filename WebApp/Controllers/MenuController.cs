using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;
using WebAPI.DTOs;
using System.Net.Http.Json;

namespace WebApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly HttpClient _http;

        public MenuController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://oicar-team-11-ajugostitelj-11.onrender.com/api/");
        }

        public async Task<IActionResult> Index(string? category)
        {
            try
            {
                var allProducts = await _http.GetFromJsonAsync<List<ProductDto>>("product");

                var categories = allProducts?
                    .Select(p => p.CategoryName)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList() ?? new();

                var filteredProducts = string.IsNullOrWhiteSpace(category)
                    ? allProducts
                    : allProducts?.Where(p => string.Equals(p.CategoryName, category, StringComparison.OrdinalIgnoreCase)).ToList();

                var viewModel = new ProductIndexViewModel
                {
                    Products = (filteredProducts ?? new()).Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        CategoryName = p.CategoryName
                    }).ToList(),
                    Categories = categories
                };

                return View("Menu", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading menu: {ex.Message}");
            }
        }
    }
}
