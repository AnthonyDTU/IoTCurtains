using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicePlatform.Models
{
    public class DeviceParameters
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceKey { get; set; }
    }
}
