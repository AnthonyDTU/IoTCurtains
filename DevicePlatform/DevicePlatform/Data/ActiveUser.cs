using DevicePlatform.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SmartDevicePlatformPlugin;
using System.Diagnostics;

namespace DevicePlatform.Data
{
    public static class ActiveUser
    {
        public static bool LoggedIn { get; set; } = false;
        public static User User { get; set; }
        public static DevicePluginCollection DevicesPlugins { get; set; }
        public static HubConnection hubConnection { get; private set; }

        private static string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        public static async Task<bool> ConfigrueSignalRHub()
        {
            // If the hub has not yet been created
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                      .WithUrl(hubUrl)
                                      .WithAutomaticReconnect()
                                      .Build();

                hubConnection.Reconnected += HubConnection_Reconnected;
            }

            // If the hub is not connected
            if (hubConnection.State != HubConnectionState.Connected)
            {
                try
                {                    
                    // Connect to the hub
                    await hubConnection.StartAsync();
                    Debug.WriteLine("Connected To Hub");

                    // Register the user with the SignalR server
                    await hubConnection.SendAsync("RegisterUser", User.UserID);
                    Debug.WriteLine("User Registered");
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


        public static async Task<bool> ConfigureActiveUser(User activeUser)
        {
            User = activeUser;
            DevicesPlugins = new DevicePluginCollection();
            LoggedIn = true;            

            if (await ConfigrueSignalRHub() == false)
            {
                return false;
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

        private static Task HubConnection_Reconnected(string arg)
        {
            // Re-register the user with the SignalR server
            return hubConnection.SendAsync("RegisterUser", User.UserID);
        }

        public static void UpdateActiveUser(User newActiveUser)
        {
            User = newActiveUser;
        }
    }
}
