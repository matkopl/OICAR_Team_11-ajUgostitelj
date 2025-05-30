using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetAll()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                Log.Information("All reviews fetched successfully!");
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetById(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);

                if (review == null)
                {
                    Log.Warning($"Review with ID {id} not found");
                    return NotFound();
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Error fetching review with ID {id}");
                return BadRequest($"Error fetching review with ID {id}, please see error log!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> Create(ReviewDTO reviewDTO)
        {
            try
            {
                Log.Information($"Creating review...");
                var createdReview = await _reviewService.CreateReviewAsync(reviewDTO);
                Log.Information($"Successfully created review  with ID:{createdReview.Id}");
                return CreatedAtAction(nameof(GetById), new { id = createdReview.Id }, createdReview);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating review");
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ReviewDTO reviewDto)
        {
            if (id != reviewDto.Id)
            {
                Log.Error($"ID in URL does not match ID in body)", id, reviewDto.Id);
                return BadRequest($"ID in URL does not match ID in body");
            }

            try
            {
                Log.Information($"Updating Review with ID {id}...");
                await _reviewService.UpdateReviewAsync(id, reviewDto);
                Log.Information($"Review with ID {id} updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating review with ID {id}");
                return StatusCode(500, "Error occurred");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Log.Information($"Deleting review ID:{id}...");
                await _reviewService.DeleteReviewAsync(id);
                Log.Information($"Successfully deleted review with ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while deleting review with ID {id}");
                return StatusCode(500, "Error with request!");
            }
        }


        [HttpGet("average/{productId}")]
        public async Task<ActionResult<double?>> GetAverageRating(int productId)
        {
            try
            {
                var avg = await _reviewService.GetAverageRatingAsync(productId);
                return Ok(avg);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to get average rating for product {productId}");
                return StatusCode(500, "Internal server error while calculating average rating");
            }
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByProductId(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }

    }
}
