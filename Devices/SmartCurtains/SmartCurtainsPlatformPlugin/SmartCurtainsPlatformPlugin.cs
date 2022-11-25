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

        private DeviceDescriptor deviceDescriptor;
        public DeviceDescriptor DeviceDescriptor => deviceDescriptor;

        private SignalRController signalRController;
        public SignalRController SignalRController => signalRController;

        DeviceData deviceData = new DeviceData();
        SmartCurtainsContentPageUI deviceContentPage;


        public delegate bool DeleteDeviceCallBack(Guid deviceID);
        public DeleteDeviceCallBack deleteDeviceCallBack;

        public SmartCurtainsPlatformPlugin(Guid userID, HubConnection hubConnection, DeleteDeviceCallBack deleteDeviceCallBack)
        {
            this.deleteDeviceCallBack = deleteDeviceCallBack;

            deviceDescriptor = new DeviceDescriptor()
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

            deviceDescriptor = new DeviceDescriptor()
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
            configurator = new SmartCurtainsConfigurator(deviceDescriptor);
            
            signalRController = new SignalRController(deviceDescriptor, 
                                                      hubConnection, 
                                                      DataReceivedFromDevice, 
                                                      DeviceAcknowledgeReceived);
            
            deviceContentPage = new SmartCurtainsContentPageUI(signalRController, 
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
            deviceContentPage.Title = deviceDescriptor.DeviceName;
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
            signalRController.RequestDeviceData();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool DeleteDevice()
        {
            signalRController.SendCommandToDevice("DeleteDevice");
            return deleteDeviceCallBack.Invoke(deviceDescriptor.DeviceID);
        }
    }
}
