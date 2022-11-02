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
        private DeviceParameters deviceParameters;
        private ConfigurationView configurationView;

        public SmartCurtainsConfigurator(DeviceParameters deviceParameters)
        {
            this.deviceParameters = deviceParameters;
        }



        public ContentView GetConfigurationView()
        {
            configurationView = new ConfigurationView(deviceParameters);
            configurationView.PopulateControls();
            return configurationView;
        }

        public ContentView GetConfigurationView(NodeConfiguration nodeConfiguration)
        {
            configurationView = new ConfigurationView(deviceParameters);
            configurationView.PopulateControls(nodeConfiguration);
            return configurationView;
        }

        public NodeConfiguration BuildNewConfiguration()
        {
            return configurationView.GetNodeConfigurationFromControls();
        }
    }
}
