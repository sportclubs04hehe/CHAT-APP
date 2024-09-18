using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        // when connect
        public override async Task OnConnectedAsync() 
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsername());
        }

        // when disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
