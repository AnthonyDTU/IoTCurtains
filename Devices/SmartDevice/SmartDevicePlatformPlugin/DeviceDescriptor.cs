using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevicePlatformPlugin
{
    public class DeviceDescriptor
    {
        public Guid DeviceID { get; set; }
        public Guid UserID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceKey { get; set; }

        public void SetDeviceDescritptor(DeviceDescriptor deviceDescriptor)
        {
            DeviceID = deviceDescriptor.DeviceID;
            UserID = deviceDescriptor.UserID;
            DeviceName = deviceDescriptor.DeviceName;
            DeviceModel = deviceDescriptor.DeviceModel;
            DeviceKey = deviceDescriptor.DeviceKey;
        }
    }
}
