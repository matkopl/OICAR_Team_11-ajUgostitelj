using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.Models;

namespace WebApp.Pages.Product
{
    public class ReviewModel : PageModel
    {
        private readonly AppDbContext _context;

        public ReviewModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Review NewReview { get; set; } = new();

        public List<Review> Reviews { get; set; } = new();
        public List<WebAPI.Models.Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            Reviews = await _context.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            Products = await _context.Products.ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            // Dohvati odabrani proizvod
            var selectedProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == NewReview.ProductId);

            if (selectedProduct == null)
            {
                ModelState.AddModelError("", "Selected product not found");
                await LoadDataAsync();
                return Page();
            }

            // Postavi ProductName automatski
            NewReview.ProductName = selectedProduct.Name;
            NewReview.ReviewDate = DateTime.UtcNow;

            _context.Reviews.Add(NewReview);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Reviews = await _context.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            Products = await _context.Products.ToListAsync();
        }
    }
}
