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
        /// 
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
        /// 
        /// </summary>
        /// <param name="deviceData"></param>
        private void SetDeviceData(DeviceData deviceData)
        {
            this.deviceData = deviceData;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        private void DataReceivedFromDevice(string jsonData)
        {
            Debug.WriteLine($"Received {jsonData} from device");
            deviceData = JsonSerializer.Deserialize<DeviceData>(jsonData);
            deviceContentPage.ConfigureData(deviceData);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeviceAcknowledgeReceived(string message)
        {
            Debug.WriteLine(message);
            deviceContentPage.DataAcknowledgedByDevice();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetCurrentDeviceState()
        {
            SignalRController.RequestDeviceData();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool DeleteDevice()
        {
            SignalRController.SendCommandToDevice("DeleteDevice");
            return deleteDeviceCallBack.Invoke(DeviceDescriptor.DeviceID);
        }
    }
}
