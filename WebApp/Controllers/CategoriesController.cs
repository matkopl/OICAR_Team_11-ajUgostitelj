using Microsoft.AspNetCore.Mvc;
using WebApp.ApiClients;

namespace WebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoriesApiClient _categoriesApiClient;

        public CategoriesController(CategoriesApiClient categoriesApiClient)
        {
            _categoriesApiClient = categoriesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoriesApiClient.LoadCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoriesApiClient.LoadCategoryAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }
    }
}
