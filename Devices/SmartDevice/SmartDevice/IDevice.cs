using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice
{
    public interface IDevice
    {
        public NodeConfiguration NodeConfiguration { get; }

        public IDeviceConfigurator DeviceConfigurator { get; }

        public ContentView GetDeviceUI(string deviceName);


    }
}
