using System;
using System.Device.Gpio;

namespace SmartDeviceFirmware
{
    /// <summary>
    /// An implemented device can inherit this to have alot of mischelanious things handled
    /// Currently handled is:
    /// * Node configuration
    /// * GPIO controller
    /// * Serial communication
    /// * WiFi
    /// * SignalR Controller
    /// * -- More to come --
    /// </summary>
    public abstract class Device
    {
        protected static GpioController gpioController;

        protected Device(string deviceModel)
        {
            NodeConfiguration.Configure(deviceModel);
        }
    }
}
