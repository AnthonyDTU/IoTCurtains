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

        //APIHandler APIHandler;        
        DeviceData currentDeviceState;

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
            GetCurrentState();
        }

        public void DataRevicedFromDevice(string data)
        {

        }


        public SmartCurtainsPlatformPlugin(DeviceDescriptor deviceDescriptor)
        {
            this.deviceDescriptor = deviceDescriptor;
        }


        public async Task<ContentView> GetPluginUI()
        {
            SmartCurtainsUI smartCurtainsUI = new SmartCurtainsUI(deviceDescriptor.DeviceName);
            currentDeviceState = new DeviceData();

            if (currentDeviceState != null)
                smartCurtainsUI.ConfigureUI(currentDeviceState);

            return smartCurtainsUI;
        }

        private async void GetCurrentState()
        {
            //currentDeviceState = await APIHandler.GetCurrentDeviceState(deviceDescriptor);
        }
    }
}
