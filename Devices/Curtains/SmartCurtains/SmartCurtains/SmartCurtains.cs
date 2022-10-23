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

        public SmartCurtains()
        {
            configurator = new SmartCurtainsConfigurator();
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


    }
}
