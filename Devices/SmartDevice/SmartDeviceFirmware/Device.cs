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
    public class Device
    {
        //protected static NodeConfiguration nodeConfiguration;
        protected static GpioController gpioController;
        //protected SerialComController serialComController;
        //protected WiFiController wifiController;
        //protected SignalRController signalRController;

        protected Device(string deviceModel)
        {
            NodeConfiguration.Configure(deviceModel);
            //nodeConfiguration = new NodeConfiguration(wifiController, signalRController, deviceModel);
        }
    }
}
