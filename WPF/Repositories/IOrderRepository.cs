using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
   
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetAllAsync(string token);
        Task<OrderDto> GetByIdAsync(string token, int orderId);
        Task<IEnumerable<OrderItemDto>> GetItemsAsync(string token, int orderId);
        Task<IEnumerable<OrderDto>> GetByTableAsync(string token, int tableId);
        Task<IEnumerable<OrderDto>> GetByPaymentMethodAsync(string token, string paymentMethod);
        Task<bool> UpdateOrderStatusAsync(string token, OrderStatusDto orderStatusDto);
        Task DeleteAsync(string token, int orderId);
    }

}
