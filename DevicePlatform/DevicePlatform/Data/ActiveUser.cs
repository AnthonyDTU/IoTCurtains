﻿using DevicePlatform.BackendControllers;
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
        public DevicePluginCollection DevicePlugins { get; private set; }
        public HubConnection hubConnection { get; private set; }
        public APIController apiController { get; private set; }


        private const string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        private static readonly Lazy<ActiveUser> lazy = new Lazy<ActiveUser>(() => new ActiveUser());

        public static ActiveUser Instance { get { return lazy.Value; } }
        private ActiveUser()
        {
            DevicePlugins = new DevicePluginCollection();
            InitializeSignalRHub();
            InitializeAPIController();

        }

        /// <summary>
        /// Initializes the API controller 
        /// </summary>
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
        /// Connects to the signal R hub
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


        /// <summary>
        /// Adds a new plugin to the active user
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public async Task AddNewDevicePlugin(IPlatformPlugin plugin)
        {
            await apiController.AddNewDevice(plugin.DeviceDescriptor);
            DevicePlugins.AddDevicePlugin(plugin);
        }


        /// <summary>
        /// Deletes a plugin from the active user
        /// </summary>
        /// <param name="plugin"></param>
        public bool RemoveDevicePlugin(Guid deviceID)
        {
            var status = apiController.DeleteDevice(User.UserID, deviceID).WaitAsync(TimeSpan.FromSeconds(10));
            DevicePlugins.DeleteDevicePlugin(deviceID);
            return true;
        }



        /// <summary>
        /// Configures the active user
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
                            DevicePlugins.AddDevicePlugin(new SmartCurtainsPlatformPlugin.SmartCurtainsPlatformPlugin(hubConnection,
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

        /// <summary>
        /// Handler for when the hub is reconnected
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task HubConnection_Reconnected(string arg)
        {
            // Re-register the user with the SignalR server
            return hubConnection.SendAsync("RegisterUser", User.UserID);
        }

        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="newActiveUser"></param>
        public void UpdateActiveUser(User newActiveUser)
        {
            User = newActiveUser;
        }
    }
}
