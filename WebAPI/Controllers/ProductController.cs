using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                Log.Information("All products fetched successfully!");
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Error fetching all tables, please see error log!");
            }
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null) 
                {
                    Log.Warning($"Product with ID {id} not found");
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Error fetching product with ID {id}");
                return BadRequest($"Error fetching product with ID {id}, please see error log!");
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductDto productDto)
        {
            try
            {
                Log.Information($"Creating product...");
                var createdProduct = await _productService.CreateProductAsync(productDto);
                Log.Information($"Successfully created {createdProduct.Name} with ID:{createdProduct.Id}");
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating product");
                return BadRequest($"Error creating product, please see error log!");
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                Log.Error($"ID in URL does not match ID in body)", id, productDto.Id);
                return BadRequest($"ID in URL does not match ID in body");
            }

            try
            {
                Log.Information($"Updating Product with ID {id}...");
                await _productService.UpdateProductAsync(id, productDto);
                Log.Information($"Product with ID {id} updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating product with ID {id}");
                return StatusCode(500, "Error occurred");
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Log.Information($"Deleting table ID:{id}...");
                await _productService.DeleteProductAsync(id);
                Log.Information($"Successfully deleted product with ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while deleting product with ID {id}");
                return StatusCode(500, "Error with request!");
            }
        }

        
        [HttpGet("byCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, "Error with request!");
            }
        }
    }
}
