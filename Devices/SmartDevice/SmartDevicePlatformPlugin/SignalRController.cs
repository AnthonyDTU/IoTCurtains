using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace SmartDevicePlatformPlugin
{
    public class SignalRController
    {
        private DeviceDescriptor deviceDescriptor;
        private HubConnection hubConnection;

        public SignalRController(DeviceDescriptor deviceDescriptor, HubConnection hubConnection, Action<string> signalRDeviceDataReceivedCallback)
        {
            this.deviceDescriptor = deviceDescriptor;
            this.hubConnection = hubConnection;

            hubConnection.Reconnected += HubConnection_Reconnected;

            // Assign callback to SmartDevice implementation
            hubConnection.On<string>("TransmitDeviceData", signalRDeviceDataReceivedCallback);

            // Register the user in the SignalR server
            hubConnection.SendAsync("RegisterUser", deviceDescriptor.UserID);
        }

        private Task HubConnection_Reconnected(string arg)
        {
            return hubConnection.SendAsync("RegisterUser", deviceDescriptor.UserID);
        }

        public void RequestDeviceData()
        {
            hubConnection.SendAsync("RequestDataFromDevice", deviceDescriptor.DeviceID);
        }

        public void TransmitDataToDevice(string jsonData)
        {
            hubConnection.SendAsync("TransmitDataToDevice", deviceDescriptor.DeviceID, jsonData);
        }
    }
}
