using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SmartDevice
{
    public interface IDeviceConfigurator
    {
        public bool SendConfigurationToDevice();

        public bool GetConfigurationFromDevice();

        public ContentView GetConfigurationView();
    
    }
}
