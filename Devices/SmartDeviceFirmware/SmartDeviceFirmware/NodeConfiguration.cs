using nanoFramework.Json;
using System;

namespace SmartDeviceFirmware
{
    public class NodeConfiguration
    {
        public string DeviceType { get; }
        public string DeviceID { get; set; }
        public string WiFiSSID { get; set; }
        public string WiFiPassword { get; set; }
        public string MACAddress { get; }
        public string IoTHubName { get; set; }
        public string SasKey { get; set; }

        public NodeConfiguration()
        {
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
