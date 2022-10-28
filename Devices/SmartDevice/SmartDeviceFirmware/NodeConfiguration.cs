using nanoFramework.Json;
using System;

namespace SmartDeviceFirmware
{
    public class NodeConfiguration
    {
        public string DeviceModel { get; set; }
        public string DeviceName { get; set; }
        public Guid DeviceID { get; set; }
        public Guid UserID { get; set; }
        public string WiFiSSID { get; set; }
        public string WiFiPassword { get; set; }
        public string MACAddress { get; set; }
        public Uri backendConnectionUri { get; set; }
        public string DeviceKey { get; set; }


        public NodeConfiguration(string DeviceModel)
        {
            this.DeviceModel = DeviceModel;

            // Check if there is a stored config, otherwise create default
        }

        public NodeConfiguration(string DeviceType, string DeviceID)
        {
                

        }

        public bool SetNewConfiguration(string config)
        {
            var newConfig = (NodeConfiguration)JsonConvert.DeserializeObject(config, typeof(NodeConfiguration));

            return false;
        }

        public bool ResetNodeToFactory()
        {
            return false;
        }

        


    }
}
