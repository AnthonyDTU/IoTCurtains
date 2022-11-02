using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCurtainsPlatformPlugin;
using SmartDevicePlatformPlugin;

namespace DevicePlatform.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual ICollection<DeviceDescriptor> DeviceDescriptors { get; set; }
    }
}
