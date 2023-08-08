using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EWS.Domain.Infra.Hubs;

public class NotificationHub : Hub
{
    public NotificationHub(IConfiguration configuration)
    {
    }
    public async Task SendNotification(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task SendNotificationByConnectionId(string connectionId, string message)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
    }

    public Task Hello()
    {
        Log.Logger.Debug(this.Context?.User?.Identity?.Name);
        Log.Logger.Debug(this.Context.ConnectionId);
        return Task.FromResult(Task.CompletedTask);
    }
}