using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SmartDevicePlatformPlugin;

namespace SmartCurtainsPlatformPlugin
{
    public class SmartCurtainsPlatformPlugin : IPlatformPlugin
    {
        private NodeConfiguration nodeConfiguration;
        public NodeConfiguration NodeConfiguration => nodeConfiguration;

        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        APIHandler APIHandler;
        DeviceParameters deviceParameters;
        DeviceData currentDeviceState;


        public SmartCurtainsPlatformPlugin(Uri backendDeviceUri)
        {
            deviceParameters = new DeviceParameters()
            {
                DeviceID = Guid.NewGuid(),
                DeviceName = "",
                DeviceKey = "newKey",
            };

            configurator = new SmartCurtainsConfigurator();
            APIHandler = new APIHandler(backendDeviceUri);
        }

        public SmartCurtainsPlatformPlugin(Uri backendDeviceUri, Guid deviceID, string deviceName, string deviceKey)
        {
            deviceParameters = new DeviceParameters()
            {
                DeviceID = deviceID,
                DeviceName = deviceName,
                DeviceKey = deviceKey,
            };

            APIHandler = new APIHandler(backendDeviceUri);
            GetCurrentState();
        }


        public SmartCurtainsPlatformPlugin(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;
        }


        public async Task<ContentView> GetDeviceUI(string deviceName)
        {
            SmartCurtainsUI smartCurtainsUI = new SmartCurtainsUI(deviceName);
            currentDeviceState = await APIHandler.GetCurrentDeviceState(deviceParameters);

            if (currentDeviceState != null)
                smartCurtainsUI.ConfigureUI(currentDeviceState);

            return smartCurtainsUI;
        }

        private async void GetCurrentState()
        {
            currentDeviceState = await APIHandler.GetCurrentDeviceState(deviceParameters);
        }
    }
}
