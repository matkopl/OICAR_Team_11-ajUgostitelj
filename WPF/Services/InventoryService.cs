using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddProductToInventoryAsync(string token, InventoryDto inventoryDto)
        {
            return await _repository.AddProductToInventoryAsync(token, inventoryDto);
        }

        public async Task<bool> ClearStockCheckHistoryAsync(string token)
        {
            return await _repository.ClearStockCheckHistoryAsync(token);
        }

        public async Task<bool> DeleteInventoryAsync(string token, int inventoryId)
        {
            return await _repository.DeleteInventoryAsync(token, inventoryId);
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync(string token)
        {
            return await _repository.GetAllInventoriesAsync(token);
        }

        public async Task<IEnumerable<StockCheckDto>> GetStockCheckHistoryAsync(string token)
        {
            return await _repository.GetStockCheckHistoryAsync(token);
        }

        public async Task<bool> PerformStockCheckAsync(string token, List<StockCheckDto> stockChecks)
        {
            return await _repository.PerformStockCheckAsync(token, stockChecks);
        }

        public async Task<bool> UpdateInventoryAsync(string token, InventoryDto inventoryDto)
        {
            return await _repository.UpdateInventoryAsync(token, inventoryDto);
        }
    }
}
