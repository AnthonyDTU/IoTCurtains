using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using SmartDevicePlatformPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Diagnostics;
using System.Xml.XPath;

namespace SmartCurtainsPlatformPlugin
{
    public class SmartCurtainsPlatformPlugin : IPlatformPlugin
    {
        private SmartCurtainsConfigurator configurator;
        public IDeviceConfigurator DeviceConfigurator => configurator;

        public DeviceDescriptor DeviceDescriptor { get; private set; }

        public SignalRController SignalRController { get; private set; }

        private DeviceData deviceData = new DeviceData();
        private SmartCurtainsContentPageUI deviceContentPage;


        public delegate bool DeleteDeviceCallBack(Guid deviceID);
        private DeleteDeviceCallBack deleteDeviceCallBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="hubConnection"></param>
        /// <param name="deleteDeviceCallBack"></param>
        public SmartCurtainsPlatformPlugin(Guid userID, HubConnection hubConnection, DeleteDeviceCallBack deleteDeviceCallBack)
        {
            this.deleteDeviceCallBack = deleteDeviceCallBack;

            DeviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = Guid.NewGuid(),
                UserID = userID,
                DeviceName = "",
                DeviceModel = "Smart Curtains",
                DeviceKey = "noKey",
            };

            InitPlugin(hubConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubConnection"></param>
        /// <param name="userID"></param>
        /// <param name="deviceID"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceKey"></param>
        /// <param name="deleteDeviceCallBack"></param>
        public SmartCurtainsPlatformPlugin(HubConnection hubConnection, Guid userID, Guid deviceID, string deviceName, string deviceKey, DeleteDeviceCallBack deleteDeviceCallBack)
        {
            this.deleteDeviceCallBack = deleteDeviceCallBack;

            DeviceDescriptor = new DeviceDescriptor()
            {
                DeviceID = deviceID,
                UserID = userID,
                DeviceName = deviceName,
                DeviceModel = "Smart Curtains",
                DeviceKey = deviceKey,
            };

            InitPlugin(hubConnection);
            GetCurrentDeviceState();
        }

        /// <summary>
        /// Initilizes the plugin
        /// </summary>
        /// <param name="hubConnection"></param>
        private void InitPlugin(HubConnection hubConnection)
        {
                configurator = new SmartCurtainsConfigurator(DeviceDescriptor);
            
                SignalRController = new SignalRController(DeviceDescriptor,
                                                          hubConnection,
                                                          DataReceivedFromDevice,
                                                          DeviceAcknowledgeReceived);

                deviceContentPage = new SmartCurtainsContentPageUI(SignalRController,
                                                                   SetDeviceData,
                                                                   DeleteDevice);
        }

        /// <summary>
        /// Sets the device data
        /// </summary>
        /// <param name="deviceData"></param>
        private void SetDeviceData(DeviceData deviceData)
        {
            this.deviceData = deviceData;
        }

        /// <summary>
        /// Gets the Main UI page for the device
        /// </summary>
        /// <returns></returns>
        public ContentPage GetPluginContentPageUI()
        {
            GetCurrentDeviceState();
            deviceContentPage.Title = DeviceDescriptor.DeviceName;
            deviceContentPage.ConfigureData(deviceData);
            return deviceContentPage;
        }


        /// <summary>
        /// Handler for when data is received from the physical device
        /// </summary>
        /// <param name="jsonData"></param>
        private void DataReceivedFromDevice(string jsonData)
        {
            Debug.WriteLine($"Received {jsonData} from device");
            deviceData = JsonSerializer.Deserialize<DeviceData>(jsonData);
            deviceContentPage.ConfigureData(deviceData);
        }

        /// <summary>
        /// Handler for when an acknowldge is reveived by the physical device
        /// </summary>
        private void DeviceAcknowledgeReceived(string message)
        {
            Debug.WriteLine(message);
            deviceContentPage.DataAcknowledgedByDevice();
        }

        /// <summary>
        /// Requests the curret device data
        /// </summary>
        private void GetCurrentDeviceState()
        {
            SignalRController.RequestDeviceData();
        }

        /// <summary>
        /// Deletes a device
        /// </summary>
        private bool DeleteDevice()
        {
            SignalRController.SendCommandToDevice("DeleteDevice");
            return deleteDeviceCallBack.Invoke(DeviceDescriptor.DeviceID);
        }
    }
}
