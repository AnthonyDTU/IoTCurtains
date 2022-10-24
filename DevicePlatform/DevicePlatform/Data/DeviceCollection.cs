using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDevice;

namespace DevicePlatform.Data
{
    public class DeviceCollection
    {
        private Dictionary<string, IDevice> devices = new Dictionary<string, IDevice>();
        public Dictionary<string, IDevice> Devices { get { return devices; } }

        List<IDevice> DevicesList { get; set; }

        public int Count => devices.Count; 

        public DeviceCollection()
        {

        }


        public void AddNewDevice(string deviceID, IDevice newDevice)
        {
            devices.Add(deviceID, newDevice);
        }

        public IDevice GetDevice(Guid deviceID)
        {
            return devices.GetValueOrDefault(deviceID.ToString());
        }



    }
}
