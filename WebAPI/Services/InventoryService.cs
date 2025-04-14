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

        public async Task<IEnumerable<InventoryDto>> GetAllInventoryItemsAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    LastUpdated = i.LastUpdated,
                    ProductName = i.Product.Name
                })
                .ToListAsync();
        }

        public async Task<InventoryDto?> GetInventoryItemByIdAsync(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                return null;

            return new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                LastUpdated = inventory.LastUpdated,
                ProductName = inventory.Product?.Name
            };
        }

        public async Task<InventoryDto?> GetInventoryByProductIdAsync(int productId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory == null)
                return null;

            return new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                LastUpdated = inventory.LastUpdated,
                ProductName = inventory.Product?.Name
            };
        }

        public async Task<InventoryDto> CreateInventoryItemAsync(InventoryDto inventoryDto)
        {
            // Provjera da li proizvod postoji
            var productExists = await _context.Products.AnyAsync(p => p.Id == inventoryDto.ProductId);
            if (!productExists)
                throw new KeyNotFoundException("Product not found");

            // Provjera da li već postoji inventar za proizvod
            var existingInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == inventoryDto.ProductId);

            if (existingInventory != null)
                throw new InvalidOperationException("Inventory already exists for this product");

            var inventory = new Inventory
            {
                ProductId = inventoryDto.ProductId,
                Quantity = inventoryDto.Quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();

            return await GetInventoryItemByIdAsync(inventory.Id) ??
                   throw new Exception("Failed to retrieve created inventory");
        }

        public async Task<InventoryDto> UpdateInventoryItemAsync(int id, InventoryDto inventoryDto)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
                throw new KeyNotFoundException("Inventory item not found");

            inventory.Quantity = inventoryDto.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            return await GetInventoryItemByIdAsync(inventory.Id) ??
                   throw new Exception("Failed to retrieve updated inventory");
        }

        public async Task DeleteInventoryItemAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
                throw new KeyNotFoundException("Inventory item not found");

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
        }
    }
}