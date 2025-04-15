using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public OrderItemService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.Product.Price
                })
                .ToListAsync();
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem == null)
                return null;

            return new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                ProductName = orderItem.Product?.Name,
                UnitPrice = orderItem.Product?.Price
            };
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.Product.Price
                })
                .ToListAsync();
        }

        public async Task<OrderItemDto> CreateOrderItemAsync(OrderItemDto orderItemDto)
        {
            // Provjera da li order i product postoje
            var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderItemDto.OrderId);
            if (!orderExists)
                throw new KeyNotFoundException("Order not found");

            var productExists = await _context.Products.AnyAsync(p => p.Id == orderItemDto.ProductId);
            if (!productExists)
                throw new KeyNotFoundException("Product not found");

            // Provjera da li već postoji order item za isti proizvod u istoj narudžbi
            var existingItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderItemDto.OrderId &&
                                          oi.ProductId == orderItemDto.ProductId);

            if (existingItem != null)
                throw new InvalidOperationException("Product already exists in this order");

            var orderItem = new OrderItem
            {
                OrderId = orderItemDto.OrderId,
                ProductId = orderItemDto.ProductId,
                Quantity = orderItemDto.Quantity
            };

            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();

            return await GetOrderItemByIdAsync(orderItem.Id) ??
                   throw new Exception("Failed to retrieve created order item");
        }

        public async Task<OrderItemDto> UpdateOrderItemAsync(int id, OrderItemDto orderItemDto)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            // Ako se mijenja ProductId, provjeri da li proizvod postoji
            if (orderItem.ProductId != orderItemDto.ProductId)
            {
                var productExists = await _context.Products.AnyAsync(p => p.Id == orderItemDto.ProductId);
                if (!productExists)
                    throw new KeyNotFoundException("New product not found");
            }

            orderItem.ProductId = orderItemDto.ProductId;
            orderItem.Quantity = orderItemDto.Quantity;

            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();

            return await GetOrderItemByIdAsync(orderItem.Id) ??
                   throw new Exception("Failed to retrieve updated order item");
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}