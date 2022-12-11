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
        internal static bool deviceIsRegistered = false;
        private static HubConnection hubConnection;
        private static string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";


        /// <summary>
        /// Configures the SignalR Controller with the specified values from an implementing device.
        /// Also tries to connect to the SignalR Hub
        /// </summary>
        /// <param name="SetDeviceDataHandler"></param>
        /// <param name="DeviceDataRequestedHandler"></param>
        /// <param name="DeviceCommandHandler"></param>
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
                RegisterDevicewithHub(NodeConfiguration.DeviceID);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could Not Connect To Hub {ex.Message}");
            }
        }


        /// <summary>
        /// Tries to open a connection to the SignalRHub
        /// </summary>
        public static void ConnectToSignalRHub()
        {
            if (!WiFiController.IsConnected)
            {
                Debug.WriteLine("WiFi not connected, cannot connect to SignalR");
                return;
            }
            
            if (hubConnection.State != HubConnectionState.Connected)
            {
                hubConnection.Start();
                Debug.WriteLine("Connected To Hub!");
                IsConnected = true;

                if (hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("Could Not Connect To Hub");
                    IsConnected = false;
                }
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
            lock (hubConnection)
            {
                if (hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("Cant transmit device data: Hub not connected");
                    return;
                }

                if (!deviceIsRegistered)
                {
                    Debug.WriteLine("Cant transmit device data: Device not registered");
                    return;
                }
                
                object[] arguments = new object[] { NodeConfiguration.UserID, NodeConfiguration.DeviceID, jsonData };
                hubConnection.SendCore("TransmitDataFromDevice", arguments);
                Debug.WriteLine($"Transmitted {jsonData} to User Platform");
            }            
        }

        /// <summary>
        /// Trasmits an acknowledge, that the device has properly recived data
        /// </summary>
        public static void TransmitDeviceAcknowledge()
        {
            lock (hubConnection)
            {
                if (hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("Cant transmit acknowledge: Hub not connected");
                    return;
                }

                if (!deviceIsRegistered)
                {
                    Debug.WriteLine("Cant transmit acknowledge: Device not registered");
                    return;
                }

                object[] arguments = new object[] { NodeConfiguration.UserID };
                //object[] arguments = new object[] { "Test Message" };
                hubConnection.SendCore("DeviceAcknowledge", arguments);
                Debug.WriteLine($"Ack Sent To User Platform, with ID {NodeConfiguration.UserID}");
            }
        }

        /// <summary>
        /// Registers the device with the SignalR Hub
        /// </summary>
        internal static void RegisterDevicewithHub(Guid deviceID)
        {
            lock (hubConnection)
            {
                if (deviceID.Equals(Guid.Empty))
                {
                    Debug.WriteLine("Cant register with hub: DeviceID not Set");
                    return;
                }

                if (hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("Cant register with hub: Hub not connected");
                    return;
                }

                hubConnection.SendCore("RegisterDevice", new object[] { deviceID });
                deviceIsRegistered = true;
                Debug.WriteLine($"Device {deviceID} Registered With HUB");                
            }           
        }


        /// <summary>
        /// Deregisters the device with the SignalR Hub
        /// </summary>
        internal static void DeregisterDeviceWithHub(Guid deviceID)
        {
            lock (hubConnection)
            {
                if (deviceID.Equals(Guid.Empty))
                {
                    Debug.WriteLine("Cant deregister with hub: DeviceID not Set");
                    return;
                }

                if (hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("Cant deregister with hub: Hub not connected");
                    return;
                }


                hubConnection.SendCore("DeregisterDevice", new object[] { deviceID });
                deviceIsRegistered = false;
                Debug.WriteLine($"Device {deviceID} Deregistered With HUB");
            }            
        }
    }
}
