using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using Windows.Storage;

namespace SmartDeviceFirmware
{
    public class NodeConfiguration
    {
        public bool IsConfigured { get; private set; }
        public string DeviceModel { get; }                      // Not configurable
        public string DeviceName { get; private set; }          // User configurable
        public Guid DeviceID { get; private set; }              // Platform configurable
        public Guid UserID { get; private set; }                // Platform configurable
        public string WiFiSSID { get; private set; }            // User configurable
        public string WiFiPassword { get; private set; }        // User configurable
        public string DeviceKey { get; private set; }           // Platform configurable

        private WiFiController wifiController;
        private SignalRController signalRController;      

        public NodeConfiguration(WiFiController wifiController, SignalRController signalRController, string DeviceModel)
        {
            this.wifiController = wifiController;
            this.signalRController = signalRController;
            this.DeviceModel = DeviceModel;

            // Check if there is a stored config, otherwise create default

            DeviceName = default;
            DeviceID = Guid.Empty;
            UserID = Guid.Empty;
            WiFiSSID = default;
            WiFiPassword = default;
            DeviceKey = default;
            IsConfigured = false;

        }
        


        public bool SetNewConfiguration(string config)
        {
            Debug.WriteLine($"Received: {config}");

            Guid oldDeviceID = DeviceID;


            try
            {
                config = config.Trim('{', '}');
                var fields = config.Split(',');

                DeviceName = fields[1].Split(':')[1].Trim('"');
                DeviceID = new Guid(fields[2].Split(':')[1].Trim('"'));
                UserID = new Guid(fields[3].Split(':')[1].Trim('"'));
                WiFiSSID = fields[4].Split(':')[1].Trim('"');
                WiFiPassword = fields[5].Split(':')[1].Trim('"');
                DeviceKey = fields[6].Split(':')[1].Trim('"');
                IsConfigured = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error inpacking config: " + ex.Message);
                return false;
            }


            // Try connect to WIFI
            wifiController.TryConnectToWiFi();

            // Deregistre old guid in hub, and register new guid in hub

            if (wifiController != null && wifiController.IsConnected && 
                signalRController != null && signalRController.IsConnected)
            {
                signalRController.DeregisterDeviceWithHub(oldDeviceID);
                signalRController.RegisterDevicewithHub(DeviceID);
            }
            return true;
        }

        public bool ResetNodeToFactory()
        {
            return false;
        }

        


    }
}
