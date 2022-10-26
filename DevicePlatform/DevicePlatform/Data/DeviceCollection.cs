using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDevicePlatformPlugin;

namespace DevicePlatform.Data
{
    public class DeviceCollection
    {
        private Dictionary<string, IPlatformPlugin> devices = new Dictionary<string, IPlatformPlugin>();
        public Dictionary<string, IPlatformPlugin> Devices { get { return devices; } }

        List<IPlatformPlugin> DevicesList { get; set; }

        public int Count => devices.Count; 

        public DeviceCollection()
        {

        }


        public void AddNewDevice(string deviceID, IPlatformPlugin newDevice)
        {
            devices.Add(deviceID, newDevice);
        }

        public IPlatformPlugin GetDevice(Guid deviceID)
        {
            return devices.GetValueOrDefault(deviceID.ToString());
        }



    }
}
