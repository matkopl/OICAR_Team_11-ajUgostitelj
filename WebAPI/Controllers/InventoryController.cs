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
            var inventory = await _inventoryService.GetAllInventoryItemsAsync();
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetById(int id)
        {
            var inventory = await _inventoryService.GetInventoryItemByIdAsync(id);
            return inventory != null ? Ok(inventory) : NotFound();
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<InventoryDto>> GetByProductId(int productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            return inventory != null ? Ok(inventory) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<InventoryDto>> Create(InventoryDto inventoryDto)
        {
            try
            {
                var createdInventory = await _inventoryService.CreateInventoryItemAsync(inventoryDto);
                return CreatedAtAction(nameof(GetById), new { id = createdInventory.Id }, createdInventory);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InventoryDto inventoryDto)
        {
            try
            {
                await _inventoryService.UpdateInventoryItemAsync(id, inventoryDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _inventoryService.DeleteInventoryItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}