using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public interface IDeviceConfigurator
    {
        /// <summary>
        /// Builds the device configuration, which can be sent to the device.
        /// </summary>
        /// <returns></returns>
        public NodeConfiguration BuildNewConfiguration();

        /// <summary>
        /// Gets the UI used for the user to input data for the device configuration.
        /// </summary>
        /// <returns></returns>
        public ContentView GetConfigurationView();

        /// <summary>
        /// Gets the UI used for the user to input data for the device configuration, based on an existing configuration.
        /// </summary>
        /// <param name="nodeConfiguration"></param>
        /// <returns></returns>
        public ContentView GetConfigurationView(NodeConfiguration nodeConfiguration);

        /// <summary>
        /// Gets the device descriptor, based on the entered configuration.
        /// </summary>
        /// <returns></returns>
        public DeviceDescriptor GetDeviceDescriptor();

    }
}
