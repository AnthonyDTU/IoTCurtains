using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCurtainsPlatformPlugin
{
    internal class DeviceParameters
    {
        public Guid DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceKey { get; set; }
    }
}
