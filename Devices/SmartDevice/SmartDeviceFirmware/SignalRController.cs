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

        public SignalRController(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;

            HubConnectionOptions options = new HubConnectionOptions()
            {
                Reconnect = true,
                KeepAliveInterval = TimeSpan.FromSeconds(10),
                //SslProtocol = System.Net.Security.SslProtocols.Tls12,
            };

            hubConnection = new HubConnection("ws://smartplatformbackendapi.azurewebsites.net/device", options: options);
            //hubConnection.On("ReceiveMessage", new[] { typeof(string), typeof(string) }, (sender, args) =>
            //{
            //    var name = (string)args[0];
            //    var message = (string)args[1];

            //    Console.WriteLine($"{name} : {message}");
            //});

            hubConnection.On("DataToDevice", new[] { typeof(string) }, HandleInconmingMessage);

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
                    Console.WriteLine("No connected To HUB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could Not Connect To Hub {ex.Message}");
            }
        }

        private void HandleInconmingMessage(object sender, object[] args)
        {
            var data = (string)args[0];

            Console.WriteLine($"{data}");
        }



    }
}
