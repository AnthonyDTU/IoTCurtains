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

        public SignalRController(NodeConfiguration nodeConfiguration)
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
            hubConnection.On("SetDeviceData", new[] { typeof(string) }, Handle_SetDeviceData);
            hubConnection.On("RequestDeviceData", new[] { typeof(string) } , Handle_RequestDeviceData);

            try
            {
                hubConnection.Start();
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    Console.WriteLine("Connected To Hub!");
                    hubConnection.SendCore("RegisterDevice", new object[] { nodeConfiguration.DeviceID });
                    Console.WriteLine("Device Registered With HUB");
                }
                else if (hubConnection.State == HubConnectionState.Disconnected)
                {
                    Console.WriteLine("Not connected To HUB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could Not Connect To Hub {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void HubConnection_Reconnecting(object sender, SignalrEventMessageArgs message)
        {
            Console.WriteLine("Disconnected from hub, trying to reconnect");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void HubConnection_Reconnected(object sender, SignalrEventMessageArgs message)
        {
            hubConnection.SendCore("RegisterDevice", new object[] { nodeConfiguration.DeviceID });
            Console.WriteLine("Reconnected to hub!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Handle_SetDeviceData(object sender, object[] args)
        {
            Console.WriteLine("Data Received");
            foreach (var item in args)
            {
                Console.Write("Recived: " + item.ToString());
            }
            
            // set device data
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Handle_RequestDeviceData(object sender, object[] args)
        {

            Console.WriteLine("Data Requested");
            Console.WriteLine(args[0].ToString());

            // call transmit Device Data, with jsonData of current and REQUESTED device data
        }

        /// <summary>
        /// Transmits data to the SignalR server
        /// Method called from device implementation
        /// </summary>
        /// <returns></returns>
        public void TransmitDeviceData(string jsonData)
        {
            object[] arguments = new object[] { nodeConfiguration.DeviceID, jsonData };
            hubConnection.SendCore("TransmitDataFromDevice", arguments);
        }
    }
}
