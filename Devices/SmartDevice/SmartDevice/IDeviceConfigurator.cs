using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SmartDevice
{
    interface IDeviceConfigurator
    {
        public bool SendConfigurationToDevice(SerialPort serialPort);

        public bool GetConfigurationFromDevice();

        public ContentView GetConfigurationView(SerialPort serialPort);
    
    }
}
