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

        public Guid DeviceID { get; }
        public string DeviceName { get; set; }
        public string DeviceKey { get; set; }

        private HttpClient backendAPI;

        public SmartCurtains(HttpClient backendAPI)
        {
            DeviceID = Guid.NewGuid();
            configurator = new SmartCurtainsConfigurator();
            this.backendAPI = backendAPI;
        }

        public SmartCurtains(Guid deviceID, string deviceName, string deviceKey, HttpClient backendAPI)
        {
            DeviceID = deviceID;
            DeviceName = deviceName;
            DeviceKey = deviceKey;
            this.backendAPI = backendAPI;

            GetCurrentState();
            // Get current state from API
        }

       


        public SmartCurtains(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;
        }



        public ContentView GetDeviceUI(string deviceName)
        {
            SmartCurtainsUI smartCurtainsUI = new SmartCurtainsUI(deviceName);

            return smartCurtainsUI;
            
        }

        private async void GetCurrentState()
        {
            var response = await backendAPI.GetAsync("");
        }


    }
}
