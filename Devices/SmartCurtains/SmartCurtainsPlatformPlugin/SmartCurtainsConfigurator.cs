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
        /// 
        /// </summary>
        /// <returns></returns>
        public ContentView GetConfigurationView()
        {
            configurationView = new ConfigurationView(deviceDescriptor);
            configurationView.PopulateControls();
            return configurationView;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public NodeConfiguration BuildNewConfiguration()
        {
            return configurationView.GetNodeConfigurationFromControls();
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceDescriptor GetDeviceDescriptor()
        {
            return configurationView.GetDeviceDescriptorFromControls();
        }
    }
}
