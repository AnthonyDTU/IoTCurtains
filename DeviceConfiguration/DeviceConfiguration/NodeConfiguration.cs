using System.Net.NetworkInformation;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DeviceConfiguration
{
    public class NodeConfiguration
    {
        public bool Configured { get; set; } = false;
        public string? MACAddress { get; set; }
        public string? WiFiSSID { get; set; } 
        public string? WiFiPassword { get; set; }
        public string? AzureIoTHub { get; set; }
        public string? DeviceID { get; set; }
        public string? SaSKey { get; set; }

    }
}