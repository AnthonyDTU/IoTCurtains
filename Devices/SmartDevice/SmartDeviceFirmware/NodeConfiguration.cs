using nanoFramework.Json;
using System;

namespace SmartDeviceFirmware
{
    public class NodeConfiguration
    {
        public string DeviceModel { get; }              // Not configurable
        public string DeviceName { get; set; }          // User configurable
        public Guid DeviceID { get; set; }              // Platform configurable
        public Guid UserID { get; set; }                // Platform configurable
        public string WiFiSSID { get; set; }            // User configurable
        public string WiFiPassword { get; set; }        // User configurable
        public string DeviceKey { get; set; }           // Platform configurable


        public NodeConfiguration(string DeviceModel)
        {
            this.DeviceModel = DeviceModel;

            // Check if there is a stored config, otherwise create default
        }

        public bool SetNewConfiguration(NodeConfiguration newNodeConfiguration)
        {
            return true;
        }

        public bool SetNewConfiguration(string config)
        {
            //var newConfig = (NodeConfiguration)JsonConvert.DeserializeObject(config, typeof(NodeConfiguration));
            //DeviceName = newConfig.DeviceName;
            //DeviceID = newConfig.DeviceID;
            //UserID = newConfig.UserID;
            //WiFiSSID = newConfig.WiFiSSID;
            //WiFiPassword = newConfig.WiFiPassword;
            //backendConnectionUri = newConfig.backendConnectionUri;
            //DeviceKey = newConfig.DeviceKey;
           
            // Try reconnect to WIFI

            return true;
        }

        public bool ResetNodeToFactory()
        {
            return false;
        }

        


    }
}
