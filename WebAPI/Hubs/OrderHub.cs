using Microsoft.AspNetCore.SignalR;
using WebAPI.DTOs;

namespace WebAPI.Hubs
{
    public class OrderHub : Hub
    {
        public async Task NotifyNewOrder(int orderId)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", orderId);
        }

        public async Task NotifyOrderStatusChange(int orderId, OrderStatus newStatus)
        {
            var statusName = Enum.GetName(typeof(OrderStatus), newStatus);
            await Clients.All.SendAsync("ReceiveOrderStatusUpdate", orderId, statusName);
        }
    }
}
