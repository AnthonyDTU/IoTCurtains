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

        private HubConnection hubConnection;

        public SmartCurtainsPlatformPlugin(Guid userID, HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = Guid.NewGuid(),
                UserID = userID,
                DeviceName = "",
                DeviceModel = "",
                DeviceKey = "",
            };

            configurator = new SmartCurtainsConfigurator(deviceDescriptor);
            signalRController = new SignalRController(deviceDescriptor, hubConnection, DataRevicedFromDevice);
            deviceContentPage = new SmartCurtainsContentPageUI();
            deviceContentPage.Disappearing += DeviceContentPage_Disappearing;
        }

        private void DeviceContentPage_Disappearing(object sender, EventArgs e)
        {
            string jsonData = JsonSerializer.Serialize<DeviceData>(deviceContentPage.GetDeviceData());
            signalRController.TransmitDataToDevice(jsonData);
            Console.WriteLine(jsonData);
        }

        public SmartCurtainsPlatformPlugin(HubConnection hubConnection, Guid userID, Guid deviceID, string deviceName, string deviceKey)
        {
            this.hubConnection = hubConnection;
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = deviceID,
                UserID = userID,
                DeviceName = deviceName,
                DeviceModel = "",
                DeviceKey = deviceKey,
            };

            signalRController = new SignalRController(deviceDescriptor, hubConnection, DataRevicedFromDevice);
            deviceContentPage = new SmartCurtainsContentPageUI();
            GetCurrentState();
        }


        public void DataRevicedFromDevice(string jsonData)
        {
            Console.WriteLine(jsonData);
            deviceData = JsonSerializer.Deserialize<DeviceData>(jsonData);
            deviceContentPage.ConfigureData(deviceData);
        }


        public SmartCurtainsPlatformPlugin(DeviceDescriptor deviceDescriptor)
        {
            this.deviceDescriptor = deviceDescriptor;
        }

        public ContentPage GetPluginContentPageUI()
        {
            deviceContentPage.Title = deviceDescriptor.DeviceName;
            deviceContentPage.ConfigureData(deviceData);
            return deviceContentPage;
        }

        private void GetCurrentState()
        {
            signalRController.RequestDeviceData();
        }
    }
}
