using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceDescriptor"></param>
        /// <param name="hubConnection"></param>
        /// <param name="signalRDeviceDataReceivedCallback"></param>
        /// <param name="signalRDeviceAcknowledgeCallback"></param>
        public SignalRController(DeviceDescriptor deviceDescriptor, HubConnection hubConnection, Action<string> signalRDeviceDataReceivedCallback, Action<string> signalRDeviceAcknowledgeCallback)
        {
            this.deviceDescriptor = deviceDescriptor;
            this.hubConnection = hubConnection;

            // Assign callback to SmartDevice implementation
            hubConnection.On($"TransmitDeviceData{deviceDescriptor.DeviceID}", signalRDeviceDataReceivedCallback);
            hubConnection.On("PassDeviceAcknowledgeToUsers", signalRDeviceAcknowledgeCallback);
        }

        /// <summary>
        /// Request data from SmartDevice
        /// </summary>
        public void RequestDeviceData()
        {
            hubConnection.SendAsync("RequestDataFromDevice", deviceDescriptor.DeviceID);
            Debug.WriteLine("Requested Device Data");
        }

        /// <summary>
        /// Transmit data to SmartDevice
        /// </summary>
        /// <param name="jsonData"></param>
        public void TransmitDataToDevice(string jsonData)
        {
            hubConnection.SendAsync("TransmitDataToDevice", deviceDescriptor.UserID, deviceDescriptor.DeviceID, jsonData);
            Debug.WriteLine($"Trasmitted: {jsonData} to device");
        }

        /// <summary>
        /// Transmits Command to SmartDevice
        /// </summary>
        /// <param name="command"></param>
        public void SendCommandToDevice(string command)
        {
            hubConnection.SendAsync("TransmitCommandToDevice", deviceDescriptor.DeviceID, command);
            Debug.WriteLine($"Transmitted command: {command} to device");
        }
    }
}
