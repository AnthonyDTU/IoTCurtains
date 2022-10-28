using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public interface IDeviceConfigurator
    {
        public void SetConfiguraion(NodeConfiguration nodeConfiguration);

        public NodeConfiguration BuildNewConfiguration();

        public ContentView GetConfigurationView();
    
    }
}
