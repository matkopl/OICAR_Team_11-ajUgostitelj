using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllInventoryItemsAsync();
        Task<InventoryDto?> GetInventoryItemByIdAsync(int id);
        Task<InventoryDto?> GetInventoryByProductIdAsync(int productId);
        Task<InventoryDto> CreateInventoryItemAsync(InventoryDto inventoryDto);
        Task<InventoryDto> UpdateInventoryItemAsync(int id, InventoryDto inventoryDto);
        Task DeleteInventoryItemAsync(int id);
    }
}
