using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice
{
    internal interface NodeConfiguration
    {
        public string DeviceModel { get; set; }

        public string IoTHubName { get; set; }
        public string DeviceID { get; set; }

        public string WiFiSSID { get; set; }
        public string WiFiPassword { get; set; }
    }
}
