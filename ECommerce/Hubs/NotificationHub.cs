using Microsoft.AspNetCore.SignalR;

namespace ECommerce.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var traderId = httpContext.Request.Query["traderId"];

            if (!string.IsNullOrEmpty(traderId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Trader_{traderId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var traderId = httpContext.Request.Query["traderId"];

            if (!string.IsNullOrEmpty(traderId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Trader_{traderId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
