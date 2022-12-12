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

        /// <summary>
        /// Gets the configuration view for the device
        /// </summary>
        /// <returns></returns>
        public ContentView GetConfigurationView()
        {
            configurationView = new ConfigurationView(deviceDescriptor);
            configurationView.PopulateControls();
            return configurationView;
        }

        /// <summary>
        /// Gets the configuration view for the device based on an existing NodeConfiguration
        /// </summary>
        /// <param name="nodeConfiguration"></param>
        /// <returns></returns>
        public ContentView GetConfigurationView(NodeConfiguration nodeConfiguration)
        {
            configurationView = new ConfigurationView(deviceDescriptor);
            configurationView.PopulateControls(nodeConfiguration);
            return configurationView;
        }

        /// <summary>
        /// Builds a NodeConfiguration based on the configuration view
        /// </summary>
        /// <returns></returns>
        public NodeConfiguration BuildNewConfiguration()
        {
            return configurationView.GetNodeConfigurationFromControls();
        }

        /// <summary>
        /// Builds a DeviceDescriptor based on the configuration view
        /// </summary>
        public DeviceDescriptor GetDeviceDescriptor()
        {
            return configurationView.GetDeviceDescriptorFromControls();
        }
    }
}
