using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public InventoryService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
        {
            var inventories = await _context.Inventories.Include(i => i.Product).ToListAsync();
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto?> GetInventoryByIdAsync(int id)
        {
            var inventoryRepo = _repositoryFactory.GetRepository<Inventory>();
            var inventory = await inventoryRepo.GetByIdAsync(id);
            return inventory != null ? _mapper.Map<InventoryDto>(inventory) : null;
        }

        public async Task<bool> AddProductToInventoryAsync(InventoryDto inventoryDto)
        {
            var inventoryRepo = _repositoryFactory.GetRepository<Inventory>();

            var existingInventory = await inventoryRepo.FindAsync(i => i.ProductId == inventoryDto.ProductId);
            if (existingInventory.Any())
            {
                return false; 
            }

            var newInventory = _mapper.Map<Inventory>(inventoryDto);
            newInventory.LastUpdated = DateTime.UtcNow;

            await inventoryRepo.AddAsync(newInventory);
            await inventoryRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateInventoryAsync(InventoryDto inventoryDto)
        {
            var inventoryRepo = _repositoryFactory.GetRepository<Inventory>();
            var inventory = await inventoryRepo.GetByIdAsync(inventoryDto.Id);

            if (inventory == null) return false;

            inventory.Quantity = inventoryDto.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            inventoryRepo.Update(inventory);
            await inventoryRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            var inventoryRepo = _repositoryFactory.GetRepository<Inventory>();
            var inventory = await inventoryRepo.GetByIdAsync(id);

            if (inventory == null) return false;

            inventoryRepo.Remove(inventory);
            await inventoryRepo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<StockCheckDto>> GetStockCheckHistoryAsync()
        {
            var stockCheckRepo = _repositoryFactory.GetRepository<StockCheck>();
            var stockChecks = await stockCheckRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<StockCheckDto>>(stockChecks);
        }

        public async Task<bool> PerformStockCheckAsync(List<StockCheckDto> stockChecks)
        {
            var stockCheckRepo = _repositoryFactory.GetRepository<StockCheck>();

            var stockCheckEntities = stockChecks.Select(stockcheck => new StockCheck
            {
                CheckDate = DateTime.UtcNow,
                ProductId = stockcheck.ProductId,
                RecordedQuantity = stockcheck.RecordedQuantity,
                ActualQuantity = stockcheck.ActualQuantity
            }).ToList();

            foreach (var stockCheck in stockCheckEntities)
            {
                await stockCheckRepo.AddAsync(stockCheck);
            }

            await stockCheckRepo.SaveChangesAsync();

            return true;
        }
    }

}