using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public class NodeConfiguration
    {
        public string DeviceType { get; }
        public string DeviceID { get; set; }
        public string WiFiSSID { get; set; }
        public string WiFiPassword { get; set; }
        public string MACAddress { get; }
        public string IoTHubName { get; set; }
        public string SasKey { get; set; }


    }
}
