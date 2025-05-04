using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("average/{productId}")]
        public async Task<ActionResult<double?>> GetAverageRating(int productId)
        {
            var avg = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();

            return Ok(avg); 
        }
    }
}
