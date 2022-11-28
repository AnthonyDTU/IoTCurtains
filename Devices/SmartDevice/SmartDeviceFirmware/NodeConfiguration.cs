using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using Windows.Storage;

namespace SmartDeviceFirmware
{
    public static class NodeConfiguration
    {
        public static bool IsConfigured { get; private set; }
        public static string DeviceModel { get; private set; }                      // Not configurable
        public static string DeviceName { get; private set; }          // User configurable
        public static Guid DeviceID { get; private set; }              // Platform configurable
        public static Guid UserID { get; private set; }                // Platform configurable
        public static string WiFiSSID { get; private set; }            // User configurable
        public static string WiFiPassword { get; private set; }        // User configurable
        public static string DeviceKey { get; private set; }           // Platform configurable

        //private static WiFiController wifiController;
        //private static SignalRController signalRController;      

        //public NodeConfiguration(WiFiController wifiController, SignalRController signalRController, string DeviceModel)
        //{
            

        //}

        public static void Configure(string deviceModel)
        {
            DeviceModel = deviceModel;

            // Check if there is a stored config, otherwise create default

            DeviceName = default;
            DeviceID = Guid.Empty;
            UserID = Guid.Empty;
            WiFiSSID = default;
            WiFiPassword = default;
            DeviceKey = default;
            IsConfigured = false;
        }
        


        public static bool SetNewConfiguration(string config)
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
            WiFiController.TryConnectToWiFi();
            
            // Deregistre old guid in hub, and register new guid in hub

            if (WiFiController.IsConnected && 
                SignalRController.IsConnected)
            {
                SignalRController.DeregisterDeviceWithHub(oldDeviceID);
                SignalRController.RegisterDevicewithHub(DeviceID);
            }
            return true;
        }

        public static bool ResetNodeToFactory()
        {
            return false;
        }
    }
}
