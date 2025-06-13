using Microsoft.AspNetCore.SignalR;



namespace WebAPI.SignalR
{
    public class OrderHub : Hub
    {
        public Task JoinOrderGroup(int orderId) {
           return  Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }
    
    }
}
