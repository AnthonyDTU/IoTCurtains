using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicePlatform.Models
{
    public class DeviceDescriptor
    {
        public Guid DeviceID { get; set; }
        public Guid UserID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceKey { get; set; }
    }
}
