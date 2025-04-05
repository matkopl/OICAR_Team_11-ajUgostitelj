using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task UpdateOrderAsync(int id, OrderDto orderDto);
        Task DeleteOrderAsync(int id);
        Task<string> GetOrderStatusAsync(int orderId);
        Task UpdateOrderStatusAsync(OrderStatusDto statusDto);
        Task<Dictionary<string, string>> GetStatusDefinitionsAsync();
    }
}
