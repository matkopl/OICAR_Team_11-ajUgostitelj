using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    // OrderService.cs
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly string _token;
        public OrderService(IOrderRepository repo, string token)
        {
            _repo = repo;
            _token = token;
        }
        public Task<IEnumerable<OrderDto>> GetAllOrdersAsync() =>
            _repo.GetAllAsync(_token);



        public Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId) =>
            _repo.GetItemsAsync(_token, orderId);

        public Task<IEnumerable<OrderDto>> GetOrdersByTableAsync(int tableId) =>
            _repo.GetByTableAsync(_token, tableId);

        public Task<IEnumerable<OrderDto>> GetOrdersByPaymentMethodAsync(string paymentMethod) =>
            _repo.GetByPaymentMethodAsync(_token, paymentMethod);

        public Task<OrderDto> GetByIdAsync(int orderId) => 
            _repo.GetByIdAsync(_token, orderId);

        public Task<bool> UpdateOrderStatusAsync(OrderStatusDto orderStatusDto) =>
            _repo.UpdateOrderStatusAsync(_token, orderStatusDto);

        public Task DeleteOrderAsync(int orderId) =>
            _repo.DeleteAsync(_token, orderId);
    }

}
