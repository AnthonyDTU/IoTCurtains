using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartDevice;

namespace SmartCurtains
{
    public class SmartCurtains : IDevice
    {
        private NodeConfiguration nodeConfiguration;
        public NodeConfiguration NodeConfiguration => nodeConfiguration;

        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        APIHandler APIHandler;
        DeviceParameters deviceParameters;
        DeviceData currentDeviceState;


        public SmartCurtains(HttpClient backendAPI)
        {
            deviceParameters = new DeviceParameters()
            {
                DeviceID = Guid.NewGuid(),
                DeviceName = "",
                DeviceKey = "newKey",
            };

            configurator = new SmartCurtainsConfigurator();
            APIHandler = new APIHandler(backendAPI);
        }

        public SmartCurtains(HttpClient backendAPI, Guid deviceID, string deviceName, string deviceKey)
        {
            deviceParameters = new DeviceParameters()
            {
                DeviceID = deviceID,
                DeviceName = deviceName,
                DeviceKey = deviceKey,
            };

            APIHandler = new APIHandler(backendAPI);
            GetCurrentState();
        }


        public SmartCurtains(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;
        }


        public ContentView GetDeviceUI(string deviceName)
        {
            SmartCurtainsUI smartCurtainsUI = new SmartCurtainsUI(deviceName);
            
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
