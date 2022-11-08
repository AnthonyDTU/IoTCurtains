using Microsoft.AspNetCore.SignalR;

namespace SmartPlatformBackendAPI.Hubs
{
    public class UserHub : Hub
    {
        private readonly static ConnectionMapping<Guid> userConnectionMapping = new ConnectionMapping<Guid>();
        

    }
}
