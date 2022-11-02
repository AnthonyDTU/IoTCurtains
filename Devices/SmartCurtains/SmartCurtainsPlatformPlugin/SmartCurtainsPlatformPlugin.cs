using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SmartDevicePlatformPlugin;
using Microsoft.Extensions.DependencyInjection;

namespace SmartCurtainsPlatformPlugin
{
    public class SmartCurtainsPlatformPlugin : IPlatformPlugin
    {
        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        private DeviceDescriptor deviceDescriptor;
        public DeviceDescriptor DeviceDescriptor => deviceDescriptor;

        APIHandler APIHandler;        
        DeviceData currentDeviceState;


        public SmartCurtainsPlatformPlugin(Guid userID, Uri backendDeviceUri)
        {
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = Guid.NewGuid(),
                UserID = userID,
                DeviceName = "",
                DeviceModel = "",
                DeviceKey = "",
                backendUri = backendDeviceUri
            };


            configurator = new SmartCurtainsConfigurator(deviceDescriptor);
            APIHandler = new APIHandler(backendDeviceUri);
        }

        public SmartCurtainsPlatformPlugin(Uri backendDeviceUri, Guid userID, Guid deviceID, string deviceName, string deviceKey)
        {
            deviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = deviceID,
                UserID = userID,
                DeviceName = deviceName,
                DeviceModel = "",
                DeviceKey = deviceKey,
                backendUri = backendDeviceUri
            };

            APIHandler = new APIHandler(backendDeviceUri);
            GetCurrentState();
        }


        public SmartCurtainsPlatformPlugin(DeviceDescriptor deviceDescriptor)
        {
            this.deviceDescriptor = deviceDescriptor;
        }


        public async Task<ContentView> GetPluginUI()
        {
            SmartCurtainsUI smartCurtainsUI = new SmartCurtainsUI(deviceDescriptor.DeviceName);
            currentDeviceState = await APIHandler.GetCurrentDeviceState(deviceDescriptor);

            if (currentDeviceState != null)
                smartCurtainsUI.ConfigureUI(currentDeviceState);

            return smartCurtainsUI;
        }

        private async void GetCurrentState()
        {
            currentDeviceState = await APIHandler.GetCurrentDeviceState(deviceDescriptor);
        }
    }
}
