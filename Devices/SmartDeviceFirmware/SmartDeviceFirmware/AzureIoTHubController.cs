using System;
using nanoFramework.Azure.Devices.Client;

namespace SmartDeviceFirmware
{
    public class AzureIoTHubController
    {
        private DeviceClient azureIoT;

        private bool isConnected;
        public bool IsConnected { get => isConnected; }


        public AzureIoTHubController(string hubName, string DeviceID, string sasKey, string moduleID = "")
        {
            try
            {
                if (moduleID == "")
                {
                    azureIoT = new DeviceClient(hubName, DeviceID, sasKey);
                }
                else
                {
                    azureIoT = new DeviceClient(hubName, DeviceID, moduleID, sasKey);
                }

                isConnected = azureIoT.Open();
            }
            catch (Exception ex)
            {

            }           
        }

        public bool GetDeviceTwin()
        {
            return false;
        }

        public bool UpdatedDeviceTwin()
        {
            return false;
        }

    }
}
