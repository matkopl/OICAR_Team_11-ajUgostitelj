// Controllers/InventoryController.cs
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("get_all")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
        {
            try
            {
                var inventory = await _inventoryService.GetAllInventoriesAsync();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToInventory(InventoryDto inventoryDto)
        {
            try
            {
                var success = await _inventoryService.AddProductToInventoryAsync(inventoryDto);
                return success ? Ok("Product added to inventory") : BadRequest("This product is already in inventory!");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateInventory(int id, InventoryDto inventoryDto)
        {
            try
            {
                inventoryDto.Id = id;
                var success = await _inventoryService.UpdateInventoryAsync(inventoryDto);
                return success ? Ok("Inventory updated successfully") : NotFound("Inventory not found");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                var success = await _inventoryService.DeleteInventoryAsync(id);
                return success ? Ok("Inventory deleted successfully") : NotFound("Inventory not found");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stock_check/history")]
        public async Task<IActionResult> GetStockCheckHistory()
        {
            try
            {
                var history = await _inventoryService.GetStockCheckHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("stock_check/perform")]
        public async Task<IActionResult> PerformStockCheck(List<StockCheckDto> stockChecks)
        {
            Console.WriteLine($"Received {stockChecks.Count} stock check items.");
            Console.WriteLine($"User Token: {Request.Headers["Authorization"]}");
            var success = await _inventoryService.PerformStockCheckAsync(stockChecks);
            return success ? Ok("Stock check recorded") : BadRequest("Failed to perform stock check");
        }

        [HttpDelete("stock_check/clear")]
        public async Task<IActionResult> ClearStockCheckHistory()
        {
            var success = await _inventoryService.ClearStockCheckHistoryAsync();
            return success ? Ok("Stock check history cleared") : BadRequest("Failed to clear stock check history");
        }
    }
}