// Controllers/InventoryController.cs
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
        {
            var inventory = await _inventoryService.GetAllInventoriesAsync();
            return Ok(inventory);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToInventory(InventoryDto inventoryDto)
        {
            var success = await _inventoryService.AddProductToInventoryAsync(inventoryDto);
            return success ? Ok("Product added to inventory") : BadRequest("Failed to add product");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateInventory(int id, InventoryDto inventoryDto)
        {
            inventoryDto.Id = id;
            var success = await _inventoryService.UpdateInventoryAsync(inventoryDto);
            return success ? Ok("Inventory updated successfully") : NotFound("Inventory not found");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var success = await _inventoryService.DeleteInventoryAsync(id);
            return success ? Ok("Inventory deleted successfully") : NotFound("Inventory not found");
        }

        [HttpGet("stock_check/history")]
        public async Task<IActionResult> GetStockCheckHistory()
        {
            var history = await _inventoryService.GetStockCheckHistoryAsync();
            return Ok(history);
        }

        [HttpPost("stock_check/perform")]
        public async Task<IActionResult> PerformStockCheck(List<StockCheckDto> stockChecks)
        {
            var success = await _inventoryService.PerformStockCheckAsync(stockChecks);
            return success ? Ok("Stock check recorded") : BadRequest("Failed to perform stock check");
        }
    }
}