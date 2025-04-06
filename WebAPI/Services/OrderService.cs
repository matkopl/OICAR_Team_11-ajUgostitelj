using AutoMapper;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public OrderService(IRepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            try
            {
                var orders = await repo.GetAllAsync();
                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching all orders in the service layer");
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            try
            {
                var order = await repo.GetByIdAsync(id);
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error fetching order with ID {id} in the service layer");
                throw;
            }
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            try
            {
                var order = _mapper.Map<Order>(orderDto);
                order.OrderDate = DateTime.UtcNow;

                await repo.AddAsync(order);
                await repo.SaveChangesAsync();

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating order in the service layer");
                throw;
            }
        }

        public async Task UpdateOrderAsync(int id, OrderDto orderDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var existingOrder = await repo.GetByIdAsync(id);

            if (existingOrder == null)
            {
                Log.Error($"Table with ID {id} not found for update in the service layer");
                throw new KeyNotFoundException("Order not found");
            }
                
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
            try
            {
                await repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error deleting order with ID {id} in the service layer");
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(OrderStatusDto statusDto)
        {
            var repo = _repositoryFactory.GetRepository<Order>();
            var order = await repo.GetByIdAsync(statusDto.OrderId);

            if (order == null) 
            {
               throw new KeyNotFoundException($"Order with ID {statusDto.OrderId} not found");
            }

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
    }
}
