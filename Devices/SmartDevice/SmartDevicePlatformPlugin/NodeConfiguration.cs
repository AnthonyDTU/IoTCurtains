using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public class NodeConfiguration
    {
        public string DeviceModel { get; set; }
        public string DeviceName { get; set; }
        public Guid DeviceID { get; set; }
        public Guid UserID { get; set; }
        public string WiFiSSID { get; set; }
        public string WiFiPassword { get; set; }
        public Uri backendConnectionUri { get; set; }
        public string DeviceKey { get; set; }

    }
}
