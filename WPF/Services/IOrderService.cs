using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Services
{
    // IOrderService.cs
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetByIdAsync(int orderId);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByTableAsync(int tableId);
        Task<IEnumerable<OrderDto>> GetOrdersByPaymentMethodAsync(string paymentMethod);
        Task<bool> UpdateOrderStatusAsync(OrderStatusDto orderStatusDto);
    }

}
