
using Microsoft.AspNetCore.SignalR.Client;

namespace EWS.BlazorWasm.Base;

public class UserStateService
{
    private readonly HubConnection _connection;
    public event Action<bool> OnUserActivityChanged;
    
    public UserStateService(HubConnection connection)
    {
        _connection = connection;
        _connection.Closed += exception =>
        {
            OnUserActivityChanged?.Invoke(false);
            return Task.CompletedTask;
        };
    }
}