using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using nanoFramework.SignalR.Client;

namespace SmartDeviceFirmware
{
    public static class SignalRController
    {
        public static bool IsConnected { get; private set; }
        private static HubConnection hubConnection;
        private static string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        //public SignalRController(NodeConfiguration nodeConfiguration, 
        //                         WiFiController wifiController,
        //                         HubConnection.OnInvokeHandler SetDeviceDataHandler, 
        //                         HubConnection.OnInvokeHandler DeviceDataRequestedHandler,
        //                         HubConnection.OnInvokeHandler DeviceCommandHandler)
        //{
        //    this.nodeConfiguration = nodeConfiguration;
        //    this.wifiController = wifiController;
        //    HubConnectionOptions options = new HubConnectionOptions()
        //    {
        //        Reconnect = true,
        //    };

        //    hubConnection = new HubConnection(hubUrl, options: options);
            
        //    // Event handlers
        //    hubConnection.Reconnecting += HubConnection_Reconnecting;
        //    hubConnection.Reconnected += HubConnection_Reconnected;

        //    // Message handlers
        //    hubConnection.On("SetDeviceData", new[] { typeof(string) }, SetDeviceDataHandler);
        //    hubConnection.On("RequestDeviceData", new[] { typeof(string) } , DeviceDataRequestedHandler);
        //    hubConnection.On("TransmitDeviceCommand", new[] { typeof(string) }, DeviceCommandHandler);
            
        //    try
        //    {
        //        ConnectToSignalRHub();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Could Not Connect To Hub {ex.Message}");
        //    }
        //}
        
        public static void Configure(HubConnection.OnInvokeHandler SetDeviceDataHandler, 
                                     HubConnection.OnInvokeHandler DeviceDataRequestedHandler,
                                     HubConnection.OnInvokeHandler DeviceCommandHandler)            
        {
            HubConnectionOptions options = new HubConnectionOptions()
            {
                Reconnect = true,
            };

            hubConnection = new HubConnection(hubUrl, options: options);

            // Event handlers
            hubConnection.Reconnecting += HubConnection_Reconnecting;
            hubConnection.Reconnected += HubConnection_Reconnected;

            // Message handlers
            hubConnection.On("SetDeviceData", new[] { typeof(string) }, SetDeviceDataHandler);
            hubConnection.On("RequestDeviceData", new[] { typeof(string) }, DeviceDataRequestedHandler);
            hubConnection.On("TransmitDeviceCommand", new[] { typeof(string) }, DeviceCommandHandler);

            try
            {
                ConnectToSignalRHub();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could Not Connect To Hub {ex.Message}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConnectToSignalRHub()
        {
            if (!WiFiController.IsConnected)
            {
                Debug.WriteLine("WiFi not connected, cannot connect to SignalR");
                return;
            }

            hubConnection.Start();
            if (hubConnection.State == HubConnectionState.Connected)
            {
                Debug.WriteLine("Connected To Hub!");
                IsConnected = true;


                if (NodeConfiguration.IsConfigured &&
                    !NodeConfiguration.DeviceID.Equals(Guid.Empty))
                {
                    RegisterDevicewithHub(NodeConfiguration.DeviceID);
                }
            }
            else if (hubConnection.State == HubConnectionState.Disconnected)
            {
                Debug.WriteLine("Not connected To HUB");
            }
        }

        /// <summary>
        /// Tells the user that the device is trying to reconnect to the hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private static void HubConnection_Reconnecting(object sender, SignalrEventMessageArgs message)
        {
            IsConnected = false;
            Debug.WriteLine("Disconnected from hub, trying to reconnect");
        }

        /// <summary>
        /// When the device has reconnected to the Hub, it must also reregister. 
        /// That is handled in the override method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private static void HubConnection_Reconnected(object sender, SignalrEventMessageArgs message)
        {
            IsConnected = false;
            hubConnection.SendCore("RegisterDevice", new object[] { NodeConfiguration.DeviceID });
            Debug.WriteLine("Reconnected to hub!");
        }

        /// <summary>
        /// Transmits data to the SignalR server
        /// Method called from device implementation
        /// </summary>
        /// <returns></returns>
        public static void TransmitDeviceData(string jsonData)
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                object[] arguments = new object[] { NodeConfiguration.UserID, NodeConfiguration.DeviceID, jsonData };
                hubConnection.SendCore("TransmitDataFromDevice", arguments);
                Debug.WriteLine($"Transmitted {jsonData} to User Platform");
            }
            else
            {
                Debug.WriteLine("Tried to transmit device data, to unconnected hub");
            }
        }

        /// <summary>
        /// Trasmits an acknowledge, that the device has properly recived data
        /// </summary>
        public static void TransmitDeviceAcknowledge()
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                object[] arguments = new object[] { NodeConfiguration.UserID };
                //object[] arguments = new object[] { "Test Message" };
                hubConnection.SendCore("DeviceAcknowledge", arguments);
                Debug.WriteLine($"Ack Sent To User Platform, with ID {NodeConfiguration.UserID}");
            }
            else
            {
                Debug.WriteLine("Tried to transmit device acknowledge, to unconnected hub");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        internal static void RegisterDevicewithHub(Guid deviceID)
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                hubConnection.SendCore("RegisterDevice", new object[] { deviceID });
                Debug.WriteLine("Device Registered With HUB");
            }
            else
            {
                Debug.WriteLine("Tried to register device with hub, but hub was not connected");
            }
        }
    

        /// <summary>
        /// 
        /// </summary>
        internal static void DeregisterDeviceWithHub(Guid deviceID)
        {
            if (deviceID.Equals(Guid.Empty))
            {
                return;
            }

            if (hubConnection.State == HubConnectionState.Connected)
            {
                hubConnection.SendCore("DeregisterDevice", new object[] { deviceID });
                Debug.WriteLine("Device Deregistered With HUB");
            }
            else
            {
                Debug.WriteLine("Tried to deregister device with hub, but hub was not connected");
            }
        }
    }
}
