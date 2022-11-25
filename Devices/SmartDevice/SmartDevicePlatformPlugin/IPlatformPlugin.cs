using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public interface IPlatformPlugin
    {
        /// <summary>
        /// The device descriptor of the device
        /// </summary>
        public DeviceDescriptor DeviceDescriptor { get; }

        /// <summary>
        /// The device configuratior
        /// </summary>
        public IDeviceConfigurator DeviceConfigurator { get; }

        /// <summary>
        /// The device's SignalR Controller 
        /// </summary>
        public SignalRController SignalRController { get; }

        /// <summary>
        /// Gets the control UI for the device
        /// </summary>
        /// <returns></returns>
        public ContentPage GetPluginContentPageUI();
    }
}
