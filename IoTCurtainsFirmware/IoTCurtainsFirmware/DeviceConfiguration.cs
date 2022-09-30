using nanoFramework.Networking;
using System;
using System.Net.NetworkInformation;
using System.Text;

namespace IoTCurtainsFirmware
{
    public class DeviceConfiguration
    {
        public bool Configured { get; set; } = false;
        public string MACAddress { get; } = NetworkInterface.GetAllNetworkInterfaces()[0].PhysicalAddress.ToString();
        public string WiFiSSID { get; set; } = "TP-LINK_0AC4EC";
        public string WiFiPassword { get; set; } = "eqh76rxg";

        // Azure Stuff:
        // ***


    }
}
