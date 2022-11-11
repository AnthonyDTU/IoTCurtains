using DevicePlatform.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace DevicePlatform.Data
{
    public static class ActiveUser
    {
        public static bool LoggedIn { get; set; } = false;
        public static User User { get; set; }
        public static DevicePluginCollection DevicesPlugins { get; set; }
        public static HubConnection hubConnection { get; private set; }

        private static string hubUrl = "http://smartplatformbackendapi.azurewebsites.net/device";


        public static async Task<bool> ConfigureActiveUser(User activeUser)
        {
            User = activeUser;
            DevicesPlugins = new DevicePluginCollection();
            LoggedIn = true;

            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                      .WithUrl(hubUrl)
                                      .WithAutomaticReconnect()
                                      .Build();

                //hubConnection.On<Guid, string>("UpdateFromDevice", (deviceID, data) =>
                //{
                //    Console.WriteLine($"Device: {deviceID} sent {data}");
                //});
            }

            if (hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await hubConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    return false;
                }
                
                await hubConnection.SendAsync("RegisterUser", User.UserID);
            }

            if (hubConnection.State == HubConnectionState.Connected &&
                User.DeviceDescriptors != null &&
                User.DeviceDescriptors.Count != 0)
            {
                foreach (var device in User.DeviceDescriptors)
                {
                    switch (device.DeviceModel)
                    {
                        case "Smart Curtains":
                            DevicesPlugins.AddNewDevicePlugin(new SmartCurtainsPlatformPlugin.SmartCurtainsPlatformPlugin(hubConnection, 
                                                                                                                          device.UserID, 
                                                                                                                          device.DeviceID, 
                                                                                                                          device.DeviceName, 
                                                                                                                          device.DeviceKey));
                            break;

                        default:
                            break;
                    }
                }
            }

            return true;
        }

        public static void UpdateActiveUser(User newActiveUser)
        {
            User = newActiveUser;
        }
    }
}
