using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartDevicePlatformPlugin;

namespace SmartCurtainsPlatformPlugin
{
    internal class SmartCurtainsConfigurator : IDeviceConfigurator
    {
        private DeviceDescriptor deviceDescriptor;
        private ConfigurationView configurationView;

        public SmartCurtainsConfigurator(DeviceDescriptor deviceDescriptor)
        {
            this.deviceDescriptor = deviceDescriptor;
        }



        public ContentView GetConfigurationView()
        {
            configurationView = new ConfigurationView(deviceDescriptor);
            configurationView.PopulateControls();
            return configurationView;
        }

        public ContentView GetConfigurationView(NodeConfiguration nodeConfiguration)
        {
            configurationView = new ConfigurationView(deviceDescriptor);
            configurationView.PopulateControls(nodeConfiguration);
            return configurationView;
        }

        public NodeConfiguration BuildNewConfiguration()
        {
            return configurationView.GetNodeConfigurationFromControls();
        }
    }
}
