using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAPI.DTOs;
using WebApp.ApiClients;

namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductApiClient _api;
        public ProductController(ProductApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var products = await _api.LoadProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await _api.LoadProductAsync(id);
            if (p is null) return NotFound();
            return View(p);
        }

        public IActionResult Create() => View(new ProductDto());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _api.CreateProductAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _api.LoadProductAsync(id);
            if (p is null) return NotFound();
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var ok = await _api.UpdateProductAsync(dto);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _api.LoadProductAsync(id);
            if (p is null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
