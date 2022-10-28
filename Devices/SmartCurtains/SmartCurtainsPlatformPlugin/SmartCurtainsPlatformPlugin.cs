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


        public SmartCurtainsPlatformPlugin(Guid UserID, Uri backendDeviceUri)
        {
            nodeConfiguration = new NodeConfiguration()
            {
                DeviceID = Guid.NewGuid(),
                UserID = UserID,
                DeviceKey = "newKey",
                backendConnectionUri = backendDeviceUri
            };


            configurator = new SmartCurtainsConfigurator(nodeConfiguration);
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
