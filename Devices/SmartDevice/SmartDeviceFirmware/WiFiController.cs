using nanoFramework.Networking;
using System;
using System.Device.Gpio;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace SmartDeviceFirmware
{
    public static class WiFiController
    {
        public static bool IsConnected { get; private set; } = false;

        private static NetworkInterface network;        
        private static int networkInterfaceIndex;
        private static int isConnectedLEDIndicatorPinNumber;
        private static GpioController gpioController;
        private static GpioPin wifiConnectedLedIndicator;

        /// <summary>
        /// Configures the WiFiController with the specified values from an implementing device.
        /// If a the node configuration has been set or loaded, the WiFiController will attempt to connect to the WiFi network.
        /// </summary>
        /// <param name="networkInterfaceIndex"></param>
        /// <param name="isConnectedLEDIndicatorPinNumber"></param>
        /// <param name="gpioController"></param>
        public static void Configure(int networkInterfaceIndex = 0, int isConnectedLEDIndicatorPinNumber = 0, GpioController gpioController = null)
        {
            WiFiController.networkInterfaceIndex = networkInterfaceIndex;
            WiFiController.isConnectedLEDIndicatorPinNumber = isConnectedLEDIndicatorPinNumber;
            WiFiController.gpioController = gpioController;
            wifiConnectedLedIndicator = gpioController.OpenPin(isConnectedLEDIndicatorPinNumber, PinMode.Output);

            network = NetworkInterface.GetAllNetworkInterfaces()[networkInterfaceIndex];

            if (network.IPv4Address != null &&
                network.IPv4Address != string.Empty)
            {
                Debug.WriteLine("Already To Wifi!");
                ConfigureConnectedToWiFi();
                return;
            }

            if (NodeConfiguration.IsConfigured &&
                NodeConfiguration.WiFiSSID != default &&
                NodeConfiguration.WiFiPassword != default)
            {
                TryConnectToWiFi();
            }
        }
        
        /// <summary>
        /// Tries to connect to a WiFi network
        /// </summary>
        internal static void TryConnectToWiFi()
        {
            try
            {
                if (WifiNetworkHelper.ConnectDhcp(NodeConfiguration.WiFiSSID,
                                                  NodeConfiguration.WiFiPassword,
                                                  WifiReconnectionKind.Automatic,
                                                  requiresDateTime: true,
                                                  wifiAdapterId: networkInterfaceIndex,
                                                  token: new CancellationTokenSource(10000).Token))
                {

                    Debug.WriteLine("Connected To Wifi!");
                    ConfigureConnectedToWiFi();
                }
                else
                {
                    Debug.WriteLine("Could Not Connect To Wifi!");
                    Debug.WriteLine($"Error: {WifiNetworkHelper.Status}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error wile connecting to WiFi:");
                Debug.WriteLine(ex.Message);
            }            
        }

        /// <summary>
        /// Sets state properties and LED indicators when connected to WiFi
        /// </summary>
        private static void ConfigureConnectedToWiFi()
        {
            IsConnected = true;
            Debug.WriteLine($"Network IP: {network.IPv4Address}");

            if (isConnectedLEDIndicatorPinNumber != 0 &&
                gpioController != null)
            {
                wifiConnectedLedIndicator.Write(PinValue.High);
            }
        }
    }
}
