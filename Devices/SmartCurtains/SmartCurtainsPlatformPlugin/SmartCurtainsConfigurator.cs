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
        private NodeConfiguration nodeConfiguration;
        private ConfigurationView configurationView;

        public SmartCurtainsConfigurator(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;
        }

        public void SetConfiguraion(NodeConfiguration nodeConfiguration)
        {
            throw new NotImplementedException();
        }

        public ContentView GetConfigurationView()
        {
            configurationView = new ConfigurationView(nodeConfiguration);
            configurationView.PopulateControls();
            return configurationView;
        }

        public NodeConfiguration BuildNewConfiguration()
        {
            nodeConfiguration = configurationView.GetNodeConfiguration();
            return nodeConfiguration;
        }
    }
}
