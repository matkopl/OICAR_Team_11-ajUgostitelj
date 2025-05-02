using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }

                await LoadDataAsync();
                return Page();
            }

            try
            {
                NewReview.ReviewDate = DateTime.UtcNow;
                _context.Reviews.Add(NewReview);
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                ModelState.AddModelError("", "Došlo je do greške pri spremanju recenzije.");
                await LoadDataAsync();
                return Page();
            }
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
