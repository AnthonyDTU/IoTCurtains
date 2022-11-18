using DevicePlatform.BackendControllers;
using DevicePlatform.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SmartDevicePlatformPlugin;
using System.Diagnostics;
using System.Net;

namespace DevicePlatform.Data
{
    public static class ActiveUser
    {
        public static bool LoggedIn { get; private set; } = false;
        public static User User { get; private set; }
        public static DevicePluginCollection DevicesPlugins { get; private set; }
        public static HubConnection hubConnection { get; private set; }
        public static APIController apiController { get; private set; }

        private static string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        public static bool InitializeUser()
        {
            DevicesPlugins = new DevicePluginCollection();
            InitializeSignalRHub();
            InitializeAPIController();




            return true;
        }



        private static void InitializeAPIController()
        {
            apiController = new APIController();
        }


        /// <summary>
        /// 
        /// </summary>
        private static void InitializeSignalRHub()
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> ConnectSignalRHub()
        {
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


        public static async void AddNewDevicePlugin(IPlatformPlugin plugin)
        {
            await apiController.AddNewDevice(plugin.DeviceDescriptor);
            DevicesPlugins.AddNewDevicePlugin(plugin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        public static async void AddDevicePlugin(IPlatformPlugin plugin)
        {
            DevicesPlugins.AddNewDevicePlugin(plugin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        public static bool RemoveDevicePlugin(Guid deviceID)
        {
            var status = apiController.DeleteDevice(User.UserID, deviceID).WaitAsync(TimeSpan.FromSeconds(10));
            //if (result.Result == HttpStatusCode.OK)
            //{
            //    DevicesPlugins.DeleteDevicePlugin(deviceID);
            //    return true;
            //}
            return true;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeUser"></param>
        /// <returns></returns>
        public static async Task<bool> ConfigureActiveUser(User activeUser)
        {
            User = activeUser;
            LoggedIn = true;            

            if (await ConnectSignalRHub() == false)
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
                                                                                                                          device.DeviceKey,
                                                                                                                          RemoveDevicePlugin));
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
