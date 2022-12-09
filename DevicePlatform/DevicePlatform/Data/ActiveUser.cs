using DevicePlatform.BackendControllers;
using DevicePlatform.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SmartDevicePlatformPlugin;
using System.Diagnostics;
using System.Net;

namespace DevicePlatform.Data
{

    public sealed class ActiveUser
    {
        public bool LoggedIn { get; private set; } = false;
        public User User { get; private set; }
        public DevicePluginCollection DevicesPlugins { get; private set; }
        public HubConnection hubConnection { get; private set; }
        public APIController apiController { get; private set; }


        private const string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        private static readonly Lazy<ActiveUser> lazy = new Lazy<ActiveUser>(() => new ActiveUser());

        public static ActiveUser Instance { get { return lazy.Value; } }
        private ActiveUser()
        {
            DevicesPlugins = new DevicePluginCollection();
            InitializeSignalRHub();
            InitializeAPIController();

        }

        private void InitializeAPIController()
        {
            apiController = new APIController();
        }

        /// <summary>
        /// Builds the connection to the SignalR hub.
        /// </summary>
        private void InitializeSignalRHub()
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
        private async Task<bool> ConnectSignalRHub()
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


        public async Task AddNewDevicePlugin(IPlatformPlugin plugin)
        {
            await apiController.AddNewDevice(plugin.DeviceDescriptor);
            DevicesPlugins.AddDevicePlugin(plugin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        public async void AddDevicePlugin(IPlatformPlugin plugin)
        {
            DevicesPlugins.AddDevicePlugin(plugin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        public bool RemoveDevicePlugin(Guid deviceID)
        {
            var status = apiController.DeleteDevice(User.UserID, deviceID).WaitAsync(TimeSpan.FromSeconds(10));
            //if (result.Result == HttpStatusCode.OK)
            //{
            //    DevicesPlugins.DeleteDevicePlugin(deviceID);
            //    return true;
            //}


            DevicesPlugins.DeleteDevicePlugin(deviceID);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeUser"></param>
        /// <returns></returns>
        public async Task<bool> ConfigureActiveUser(User activeUser)
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
                            DevicesPlugins.AddDevicePlugin(new SmartCurtainsPlatformPlugin.SmartCurtainsPlatformPlugin(hubConnection,
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

        private Task HubConnection_Reconnected(string arg)
        {
            // Re-register the user with the SignalR server
            return hubConnection.SendAsync("RegisterUser", User.UserID);
        }

        public void UpdateActiveUser(User newActiveUser)
        {
            User = newActiveUser;
        }
    }
}
