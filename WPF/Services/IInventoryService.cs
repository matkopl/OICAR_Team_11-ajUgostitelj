using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync(string token);
        Task<bool> AddProductToInventoryAsync(string token, InventoryDto inventoryDto);
        Task<bool> UpdateInventoryAsync(string token, InventoryDto inventoryDto);
        Task<bool> DeleteInventoryAsync(string token, int inventoryId);
        Task<IEnumerable<StockCheckDto>> GetStockCheckHistoryAsync(string token);
        Task<bool> PerformStockCheckAsync(string token, List<StockCheckDto> stockChecks);
        Task<bool> ClearStockCheckHistoryAsync(string token);
    }
}
