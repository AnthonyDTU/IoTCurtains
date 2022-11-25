using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using Windows.Storage;

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


        public bool createConfigFile()
        {
            try
            {
                

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting path: " + ex.Message);
            }

            return true;
        }

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
            Debug.WriteLine($"Received: {config}");

            config = config.Trim('{', '}');
            var fields = config.Split(',');

            DeviceName = fields[1].Split(':')[1].Trim('"');
            DeviceID = new Guid(fields[2].Split(':')[1].Trim('"'));
            UserID = new Guid(fields[3].Split(':')[1].Trim('"'));
            WiFiSSID = fields[4].Split(':')[1].Trim('"');
            WiFiPassword = fields[5].Split(':')[1].Trim('"');
            DeviceKey = fields[6].Split(':')[1].Trim('"');

           
            // Try reconnect to WIFI
            // Connect and configure hub    

            return true;
        }

        public bool ResetNodeToFactory()
        {
            return false;
        }

        


    }
}
