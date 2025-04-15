using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync();
        Task<OrderItemDto?> GetOrderItemByIdAsync(int id);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<OrderItemDto> CreateOrderItemAsync(OrderItemDto orderItemDto);
        Task<OrderItemDto> UpdateOrderItemAsync(int id, OrderItemDto orderItemDto);
        Task DeleteOrderItemAsync(int id);
    }
}
