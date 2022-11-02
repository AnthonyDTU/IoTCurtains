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
        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        private DeviceParameters deviceParameters;
        public DeviceParameters DeviceParameters => deviceParameters;

        APIHandler APIHandler;        
        DeviceData currentDeviceState;


        public SmartCurtainsPlatformPlugin(Guid userID, Uri backendDeviceUri)
        {
            deviceParameters = new DeviceParameters()
            {
                DeviceID = Guid.NewGuid(),
                UserID = userID,
                DeviceName = "",
                DeviceModel = "",
                DeviceKey = "newKey",
                backendUri = backendDeviceUri
            };


            configurator = new SmartCurtainsConfigurator(deviceParameters);
            APIHandler = new APIHandler(backendDeviceUri);
        }

        public SmartCurtainsPlatformPlugin(Uri backendDeviceUri, Guid userID, Guid deviceID, string deviceName, string deviceKey)
        {
            deviceParameters = new DeviceParameters()
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


        public SmartCurtainsPlatformPlugin(DeviceParameters deviceParameters)
        {
            this.deviceParameters = deviceParameters;
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
