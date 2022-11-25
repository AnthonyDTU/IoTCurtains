using nanoFramework.Networking;
using System;
using System.Device.Gpio;
using System.Device.Wifi;
using System.Net.NetworkInformation;
using System.Threading;

namespace SmartDeviceFirmware
{
    public class WiFiController
    {
        private bool isConnected = false;
        public bool IsConnected { get { return isConnected; } }

        private NetworkInterface network;

        private string SSID;
        private string password;
        private readonly int networkInterfaceIndex;
        private readonly int isConnectedLEDIndicatorPinNumber;
        private readonly GpioController gpioController;
        private GpioPin wifiConnectedLedIndicator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="password"></param>
        /// <param name="networkInterfaceIndex"></param>
        /// <param name="isConnectedLEDIndicatorPinNumber"></param>
        public WiFiController(NodeConfiguration nodeConfiguration,  int networkInterfaceIndex = 0, int isConnectedLEDIndicatorPinNumber = 0, GpioController gpioController = null)
        {
            SSID = nodeConfiguration.WiFiSSID;
            password = nodeConfiguration.WiFiPassword;
            this.networkInterfaceIndex = networkInterfaceIndex;
            this.isConnectedLEDIndicatorPinNumber = isConnectedLEDIndicatorPinNumber;
            this.gpioController = gpioController;

            network = NetworkInterface.GetAllNetworkInterfaces()[networkInterfaceIndex];
            if (network.IPv4Address != null &&
                network.IPv4Address != string.Empty)
            {
                TryConnectToWiFi();
            }
            else
            {
                isConnected = true;
            }
        }

        /// <summary>
        /// Updates the SSID and password, and tries to connected to the new WiFi
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="password"></param>
        internal void SetAndConnectNewWifi(string SSID, string password)
        {
            this.SSID = SSID;
            this.password = password;
            TryConnectToWiFi();
        }

        /// <summary>
        /// Tries to connect to a WiFi network
        /// </summary>
        private void TryConnectToWiFi()
        {
            try
            {
                if (WifiNetworkHelper.ConnectDhcp(SSID,
                                                  password,
                                                  WifiReconnectionKind.Automatic,
                                                  requiresDateTime: true,
                                                  wifiAdapterId: networkInterfaceIndex,
                                                  token: new CancellationTokenSource(10000).Token))
                {
                    isConnected = true;

                    Console.WriteLine("Connected To Wifi!");
                    Console.WriteLine($"Network IP: {network.IPv4Address}");

                    if (isConnectedLEDIndicatorPinNumber != 0 &&
                        gpioController != null)
                    {
                        wifiConnectedLedIndicator = gpioController.OpenPin(isConnectedLEDIndicatorPinNumber, PinMode.Output);
                        wifiConnectedLedIndicator.Write(PinValue.High);
                    }
                }
                else
                {
                    Console.WriteLine("Could Not Connect To Wifi!");
                    Console.WriteLine($"Error: {WifiNetworkHelper.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error wile connecting to WiFi:");
                Console.WriteLine(ex.Message);
            }            
        }
    }
}
