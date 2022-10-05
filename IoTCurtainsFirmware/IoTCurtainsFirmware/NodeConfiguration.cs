using System;
using System.Text;

namespace IoTCurtainsFirmware
{
    internal class NodeConfiguration
    {
        public bool Configured { get; set; } = false;
        public string? MACAddress { get; set; }
        public string? WiFiSSID { get; set; }
        public string? WiFiPassword { get; set; }
        public string? AzureIoTHub { get; set; }
        public string? DeviceID { get; set; }
        public string? SaSKey { get; set; }

    }
}
