using System;
using System.Device.Gpio;

namespace SmartDeviceFirmware
{
    public class Device
    {
        protected NodeConfiguration nodeConfiguration;
        protected GpioController gpioController;
        protected SerialCommunicator serialCommunicator;
        protected WiFiHandler wifiHandler;

        public Device(string deviceModel)
        {
            nodeConfiguration = new NodeConfiguration(deviceModel);
        }
    }
}
