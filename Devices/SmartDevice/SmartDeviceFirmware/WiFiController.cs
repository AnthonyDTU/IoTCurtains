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
        public static bool IsConnected { get; private set; }

        private static NetworkInterface network;
        //private readonly NodeConfiguration nodeConfiguration;
        //private readonly SignalRController signalRController;
        
        private static int networkInterfaceIndex;
        private static int isConnectedLEDIndicatorPinNumber;
        private static GpioController gpioController;
        private static GpioPin wifiConnectedLedIndicator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="password"></param>
        /// <param name="networkInterfaceIndex"></param>
        /// <param name="isConnectedLEDIndicatorPinNumber"></param>
        //public WiFiController(NodeConfiguration nodeConfiguration, SignalRController signalRController, int networkInterfaceIndex = 0, int isConnectedLEDIndicatorPinNumber = 0, GpioController gpioController = null)
        //{
        //    this.nodeConfiguration = nodeConfiguration;
        //    this.signalRController = signalRController;
        //    this.networkInterfaceIndex = networkInterfaceIndex;
        //    this.isConnectedLEDIndicatorPinNumber = isConnectedLEDIndicatorPinNumber;
        //    this.gpioController = gpioController;
           
        //    network = NetworkInterface.GetAllNetworkInterfaces()[networkInterfaceIndex];
        //    if (nodeConfiguration != null &&
        //        nodeConfiguration.IsConfigured &&
        //        nodeConfiguration.WiFiSSID != null && 
        //        nodeConfiguration.WiFiSSID != default &&
        //        nodeConfiguration.WiFiPassword != null)
        //    {
        //        TryConnectToWiFi();
        //    }

        //    if (network.IPv4Address != null &&
        //        network.IPv4Address != string.Empty)
        //    {
        //        isConnected = true;
        //    }
        //}

        public static void Configure(int networkInterfaceIndex = 0, int isConnectedLEDIndicatorPinNumber = 0, GpioController gpioController = null)
        {
            WiFiController.networkInterfaceIndex = networkInterfaceIndex;
            WiFiController.isConnectedLEDIndicatorPinNumber = isConnectedLEDIndicatorPinNumber;
            WiFiController.gpioController = gpioController;

            network = NetworkInterface.GetAllNetworkInterfaces()[networkInterfaceIndex];
            if (NodeConfiguration.IsConfigured &&
                NodeConfiguration.WiFiSSID != null &&
                NodeConfiguration.WiFiSSID != default &&
                NodeConfiguration.WiFiPassword != null)
            {
                TryConnectToWiFi();
            }

            if (network.IPv4Address != null &&
                network.IPv4Address != string.Empty)
            {
                IsConnected = true;
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
                    IsConnected = true;

                    Debug.WriteLine("Connected To Wifi!");
                    Debug.WriteLine($"Network IP: {network.IPv4Address}");

                    if (isConnectedLEDIndicatorPinNumber != 0 &&
                        gpioController != null)
                    {
                        wifiConnectedLedIndicator = gpioController.OpenPin(isConnectedLEDIndicatorPinNumber, PinMode.Output);
                        wifiConnectedLedIndicator.Write(PinValue.High);
                    }
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
    }
}
