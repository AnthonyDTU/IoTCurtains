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
    /// * -- API Controller
    /// * -- 
    /// </summary>
    public class Device
    {
        protected readonly NodeConfiguration nodeConfiguration;
        protected GpioController gpioController;
        protected SerialCommunicator serialCommunicator;
        protected WiFiHandler wifiHandler;
        protected SignalRController signalRController;

        protected Device(string deviceModel)
        {
            nodeConfiguration = new NodeConfiguration(deviceModel);
        }
    }
}
