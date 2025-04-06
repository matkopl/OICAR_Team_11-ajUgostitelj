using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using System.Linq.Dynamic.Core;

namespace WebAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public OrderService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var orders = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = await repo.GetByIdAsync(id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = _mapper.Map<Order>(orderDto);
            order.OrderDate = DateTime.UtcNow;

            await repo.AddAsync(order);
            await repo.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrderAsync(int id, OrderDto orderDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var existingOrder = await repo.GetByIdAsync(id);

            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found");

            _mapper.Map(orderDto, existingOrder);
            repo.Update(existingOrder);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = await repo.GetByIdAsync(id);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            repo.Remove(order);
            await repo.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(OrderStatusDto statusDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = await repo.GetByIdAsync(statusDto.OrderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {statusDto.OrderId} not found");

            if (!Enum.TryParse<OrderStatus>(statusDto.Status, out var newStatus))
                throw new ArgumentException($"Invalid status value: {statusDto.Status}");

            // Logika za validne status tranzicije
            if (order.Status == OrderStatus.Completed && newStatus != OrderStatus.Paid)
                throw new InvalidOperationException("Completed orders can only transition to Paid status");

            order.Status = newStatus;
            repo.Update(order);
            await repo.SaveChangesAsync();
        }

        public async Task<string> GetOrderStatusAsync(int orderId)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = await repo.GetByIdAsync(orderId);
            return order?.Status.ToString() ?? throw new KeyNotFoundException("Order not found");
        }

        public async Task<Dictionary<string, string>> GetStatusDefinitionsAsync()
        {
            return await Task.FromResult(new Dictionary<string, string>
        {
            { "Pending", "Narudžba kreirana, čeka obradu" },
            { "InProgress", "Narudžba je u pripremi" },
            { "Completed", "Narudžba je završena" },
            { "Cancelled", "Narudžba je otkazana" },
            { "Paid", "Narudžba je plaćena" }
        });
        }

        public async Task<(List<OrderDto> Orders, int TotalCount)> GetOrdersPagedAsync(OrderQueryDto query)
        {
            // Inicijalizacija ako je query null
            query ??= new OrderQueryDto();

            // Validacija parametara
            query.Page = Math.Max(1, query.Page);
            query.PageSize = new[] { 5, 10, 15 }.Contains(query.PageSize)
                ? query.PageSize
                : 5;

            // Osnovni upit sa osiguranjem da nije null
            IQueryable<Order> baseQuery = _context.Orders?
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable() ?? throw new Exception("Database context is not initialized");

            // Sortiranje sa null check
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var validColumns = new[] { "Status", "TotalAmount", "TableId", "OrderDate" };
                if (validColumns.Contains(query.SortBy))
                {
                    var sortDirection = query.SortDescending ? "descending" : "ascending";
                    try
                    {
                        baseQuery = baseQuery.OrderBy($"{query.SortBy} {sortDirection}");
                    }
                    catch
                    {
                        // Fallback na default sort
                        baseQuery = baseQuery.OrderByDescending(o => o.OrderDate);
                    }
                }
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(o => o.OrderDate);
            }

            // Paginacija sa null check
            var totalCount = await baseQuery.CountAsync();
            var orders = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync() ?? new List<Order>();

            return (_mapper.Map<List<OrderDto>>(orders), totalCount);
        }

        public List<int> GetAvailablePageSizes()
        {
            return new List<int> { 5, 10, 15 };
        }

        public List<string> GetAvailableSortColumns()
        {
            return new List<string>
            {
                "Status",
                "TotalAmount",
                "TableId",
                "OrderDate"
            };
        }
    }
}
