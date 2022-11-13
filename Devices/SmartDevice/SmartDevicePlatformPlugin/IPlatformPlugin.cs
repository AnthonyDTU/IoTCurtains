using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public interface IPlatformPlugin
    {
        public DeviceDescriptor DeviceDescriptor { get; }

        public IDeviceConfigurator DeviceConfigurator { get; }

        public SignalRController SignalRController { get; }

        public ContentPage GetPluginContentPageUI();


    }
}
