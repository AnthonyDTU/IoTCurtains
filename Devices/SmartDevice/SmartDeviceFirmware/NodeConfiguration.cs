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
        public static string DeviceModel { get; private set; }         // Not configurable
        public static string DeviceName { get; private set; }          // User configurable
        public static Guid DeviceID { get; private set; }              // Platform configurable
        public static Guid UserID { get; private set; }                // Platform configurable
        public static string WiFiSSID { get; private set; }            // User configurable
        public static string WiFiPassword { get; private set; }        // User configurable
        public static string DeviceKey { get; private set; }           // Platform configurable

        /// <summary>
        /// Configures the node with either the default configuration or the configuration from the file system if one exists (not implemented yet)
        /// </summary>
        /// <param name="deviceModel"></param>
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


        /// <summary>
        /// Sets a new configuration for the device. 
        /// The configuration is based on a config string recived from the ConfigManager on the platform.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
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
                Debug.WriteLine("Error unpacking config: " + ex.Message);
                return false;
            }


            // Try connect to WIFI
            WiFiController.TryConnectToWiFi();

            if (!SignalRController.IsConnected)
            {
                SignalRController.ConnectToSignalRHub();
            }

            if (!SignalRController.deviceIsRegistered)
            {
                SignalRController.RegisterDevicewithHub(DeviceID);
            }

            // Deregistre old guid in hub, and register new guid in hub

            if (WiFiController.IsConnected && 
                SignalRController.IsConnected)
            {
                SignalRController.DeregisterDeviceWithHub(oldDeviceID);
                SignalRController.RegisterDevicewithHub(DeviceID);
            }
            return true;
        }

        /// <summary>
        /// Resets the configuration to default
        /// </summary>
        public static void ResetNodeToFactory()
        {
            DeviceName = default;
            DeviceID = Guid.Empty;
            UserID = Guid.Empty;
            WiFiSSID = default;
            WiFiPassword = default;
            DeviceKey = default;
            IsConfigured = false;
        }
    }
}
