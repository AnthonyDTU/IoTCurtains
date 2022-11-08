using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.SignalR;


namespace SmartPlatformBackendAPI.Hubs
{
    public class DeviceHub : Hub
    {
        private readonly static ConnectionMapping<Guid> _connections = new ConnectionMapping<Guid>();
        private readonly static ConnectionMapping<Guid> userConnectionMapping = new ConnectionMapping<Guid>();


        public void SendDataToDevice(Guid deviceID, string jsonData)
        {
            foreach (var connectionId in _connections.GetConnections(deviceID))
            {
                Clients.Client(connectionId).SendAsync("DataToDevice", jsonData);
            }
        }

        public Guid RegisterDevice(Guid deviceID)
        {
            _connections.Add(deviceID, Context.ConnectionId);
            return deviceID;
        }

        public Guid RegisterUser(Guid userID)
        {
            _connections.Add(userID, Context.ConnectionId);
            return userID;
        }

        public Task AwaitCientConnected()
        {
            return Clients.Caller.SendAsync("AwaitCientConnected", true);
        }
    }
}
