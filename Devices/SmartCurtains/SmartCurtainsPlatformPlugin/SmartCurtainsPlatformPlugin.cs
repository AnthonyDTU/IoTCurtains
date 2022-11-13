using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SmartDevicePlatformPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Diagnostics;

namespace SmartCurtainsPlatformPlugin
{
    public class SmartCurtainsPlatformPlugin : IPlatformPlugin
    {
        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        private DeviceDescriptor deviceDescriptor;
        public DeviceDescriptor DeviceDescriptor => deviceDescriptor;

        private SignalRController signalRController;
        public SignalRController SignalRController => signalRController;

        DeviceData deviceData = new DeviceData();
        SmartCurtainsContentPageUI deviceContentPage;
        
        public SmartCurtainsPlatformPlugin(Guid userID, HubConnection hubConnection)
        {
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = Guid.NewGuid(),
                UserID = userID,
                DeviceName = "",
                DeviceModel = "Smart Curtains",
                DeviceKey = "",
            };

            InitPlugin(hubConnection);
        }

        
        public SmartCurtainsPlatformPlugin(HubConnection hubConnection, Guid userID, Guid deviceID, string deviceName, string deviceKey)
        {
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = deviceID,
                UserID = userID,
                DeviceName = deviceName,
                DeviceModel = "Smart Curtains",
                DeviceKey = deviceKey,
            };

            InitPlugin(hubConnection);
            GetCurrentDeviceState();
        }

        private void InitPlugin(HubConnection hubConnection)
        {
            configurator = new SmartCurtainsConfigurator(deviceDescriptor);
            signalRController = new SignalRController(deviceDescriptor, hubConnection, DataReceivedFromDevice, DeviceAcknowledgeReceived);
            deviceContentPage = new SmartCurtainsContentPageUI(signalRController);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ContentPage GetPluginContentPageUI()
        {
            GetCurrentDeviceState();
            deviceContentPage.Title = deviceDescriptor.DeviceName;
            deviceContentPage.ConfigureData(deviceData);
            return deviceContentPage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void SendDataToDevice(object sender, EventArgs e)
        //{
        //    string jsonData = JsonSerializer.Serialize<DeviceData>(deviceContentPage.GetDeviceData());
        //    signalRController.TransmitDataToDevice(jsonData);
        //    Console.WriteLine(jsonData);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        public void DataReceivedFromDevice(string jsonData)
        {
            Debug.WriteLine($"Received {jsonData} from device");
            deviceData = JsonSerializer.Deserialize<DeviceData>(jsonData);
            deviceContentPage.ConfigureData(deviceData);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeviceAcknowledgeReceived(string message)
        {
            Debug.WriteLine(message);

            deviceContentPage.DataAcknowledgedByDevice();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetCurrentDeviceState()
        {
            signalRController.RequestDeviceData();
        }
    }
}
