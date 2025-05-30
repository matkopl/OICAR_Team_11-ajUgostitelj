using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto?> GetInventoryByIdAsync(int id);
        Task<bool> AddProductToInventoryAsync(InventoryDto inventoryDto);
        Task<bool> UpdateInventoryAsync(InventoryDto inventoryDto);
        Task<bool> DeleteInventoryAsync(int id);
        Task<IEnumerable<StockCheckDto>> GetStockCheckHistoryAsync();
        Task<bool> PerformStockCheckAsync(List<StockCheckDto> stockChecks);
        Task<bool> ClearStockCheckHistoryAsync();
    }
}
