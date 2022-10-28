using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDevicePlatformPlugin;

namespace DevicePlatform.Data
{
    public class DevicePluginCollection
    {
        private Dictionary<string, IPlatformPlugin> devices = new Dictionary<string, IPlatformPlugin>();
        public Dictionary<string, IPlatformPlugin> Plugins { get { return devices; } }

        List<IPlatformPlugin> DevicesList { get; set; }

        public int Count => devices.Count; 

        public DevicePluginCollection()
        {

        }


        public void AddNewDevicePlugin(string deviceID, IPlatformPlugin newDevice)
        {
            devices.Add(deviceID, newDevice);
        }

        public IPlatformPlugin GetDevice(Guid deviceID)
        {
            return devices.GetValueOrDefault(deviceID.ToString());
        }



    }
}
