using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public interface IPlatformPlugin
    {
        public DeviceParameters DeviceParameters { get; }

        public IDeviceConfigurator DeviceConfigurator { get; }

        public Task<ContentView> GetDeviceUI(string deviceName);


    }
}
