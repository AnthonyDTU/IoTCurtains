using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using nanoFramework.SignalR.Client;

namespace SmartDeviceFirmware
{
    public class SignalRController
    {
        HubConnection hubConnection;
        NodeConfiguration nodeConfiguration;
        string hubUrl = "ws://smartplatformbackendapi.azurewebsites.net/device";

        public SignalRController(NodeConfiguration nodeConfiguration, 
                                 HubConnection.OnInvokeHandler SetDeviceDataHandler, 
                                 HubConnection.OnInvokeHandler DeviceDataRequestedHandler,
                                 HubConnection.OnInvokeHandler DeviceCommandHandler)
        {
            this.nodeConfiguration = nodeConfiguration;
            HubConnectionOptions options = new HubConnectionOptions()
            {
                Reconnect = true,
                //KeepAliveInterval = TimeSpan.FromSeconds(10),
            };

            hubConnection = new HubConnection(hubUrl, options: options);
            
            // Event handlers
            hubConnection.Reconnecting += HubConnection_Reconnecting;
            hubConnection.Reconnected += HubConnection_Reconnected;

            // Message handlers
            hubConnection.On("SetDeviceData", new[] { typeof(string) }, SetDeviceDataHandler);
            hubConnection.On("RequestDeviceData", new[] { typeof(string) } , DeviceDataRequestedHandler);
            hubConnection.On("TransmitDeviceCommand", new[] { typeof(string) }, DeviceCommandHandler);

            try
            {
                hubConnection.Start();
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    Debug.WriteLine("Connected To Hub!");
                    hubConnection.SendCore("RegisterDevice", new object[] { nodeConfiguration.DeviceID });
                    Debug.WriteLine("Device Registered With HUB");
                }
                else if (hubConnection.State == HubConnectionState.Disconnected)
                {
                    Debug.WriteLine("Not connected To HUB");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could Not Connect To Hub {ex.Message}");
            }
        }

        /// <summary>
        /// Tells the user that the device is trying to reconnect to the hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void HubConnection_Reconnecting(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine("Disconnected from hub, trying to reconnect");
        }

        /// <summary>
        /// When the device has reconnected to the Hub, it must also reregister. 
        /// That is handled in the override method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void HubConnection_Reconnected(object sender, SignalrEventMessageArgs message)
        {
            hubConnection.SendCore("RegisterDevice", new object[] { nodeConfiguration.DeviceID });
            Debug.WriteLine("Reconnected to hub!");
        }


        /// <summary>
        /// Transmits data to the SignalR server
        /// Method called from device implementation
        /// </summary>
        /// <returns></returns>
        public void TransmitDeviceData(string jsonData)
        {
            object[] arguments = new object[] { nodeConfiguration.UserID, jsonData };
            hubConnection.SendCore("TransmitDataFromDevice", arguments);
            Debug.WriteLine($"Transmitted {jsonData} to User Platform");
        }

        /// <summary>
        /// Trasmits an acknowledge, that the device has properly recived data
        /// </summary>
        public void TransmitDeviceAcknowledge()
        {
            object[] arguments = new object[] { nodeConfiguration.UserID };
            //object[] arguments = new object[] { "Test Message" };
            hubConnection.SendCore("DeviceAcknowledge", arguments);
            Debug.WriteLine($"Ack Sent To User Platform, with ID {nodeConfiguration.UserID}");
        }
    }
}
