﻿using System;
using System.IO.Ports;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;

namespace SmartDeviceFirmware
{
    public class SerialComController
    {
        SerialPort serialPort;
        int baudRate = 115200;
        Parity parity = Parity.None;
        int databits = 8;
        StopBits stopBits = StopBits.One;

        private const string getConfigCommand = "deviceConfig?";
        private const string setConfigCommand = "config";
        private const string newConfigHeader = "newConfig:";

        private const string getDeviceModelQuery = "deviceModel?";
        private const string readyForConfigQuery = "readyForConfig?";
        private const string readyForConfigResponse = "readyForConfig!";
        private const string deviceConfiguredCommand = "deviceConfigured!";
        private const string resetNodeCommand = "resetNode";

        private const string setDeviceNameCommand = "deviceName:";
        private const string setDeviceIDCommand = "deviceID:";
        private const string setUserIDCommand = "userID:";
        private const string setWiFiSSIDCommand = "WiFiSSID:";
        private const string setWiFiPasswordCommand = "WiFiPassword:";
        private const string setDeviceKeyCommand = "deviceKey:";


        public delegate void DataRecivedHandler(string data);
        public DataRecivedHandler dataRecivedCallbackHandler;

        private NodeConfiguration nodeConfiguration;

        /// <summary>
        /// Constructer which initilizes the pins and the serial port
        /// </summary>
        /// <param name="COMPort"></param>
        /// <param name="rxPinNumber"></param>
        /// <param name="txPinNumber"></param>
        /// <param name="dataRecivedHandler">Passes back all data, that is not related to config commands</param>
        public SerialComController(NodeConfiguration nodeConfiguration, string COMPort, int rxPinNumber, int txPinNumber, DataRecivedHandler dataRecivedCallbackHandler = null)
        {
            this.nodeConfiguration = nodeConfiguration;
            this.dataRecivedCallbackHandler = dataRecivedCallbackHandler;

            Configuration.SetPinFunction(rxPinNumber, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(txPinNumber, DeviceFunction.COM2_TX);

            serialPort = new SerialPort(COMPort,
                                        baudRate, 
                                        parity, 
                                        databits, 
                                        stopBits);

            serialPort.ReadBufferSize = 2048;
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        /// <summary>
        /// Handler for when new data is received on the serialport
        /// A passback function can be set up during construction of this class, to pass back data that is not config related
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string recievedData = serialPort.ReadLine();
            Debug.WriteLine($"Recived String: {recievedData}");

            // If config is requested, transmit it to the requester
            if (recievedData == getConfigCommand)
            {

                throw new NotImplementedException();
                //serialPort.WriteLine(JsonConvert.SerializeObject(nodeConfiguration));
                //return;
            }

            // If the platform asks if the device is ready for config
            if (recievedData == readyForConfigQuery)
            {
                serialPort.WriteLine(readyForConfigResponse);
                return;
            }

            // If a new configuration is detected
            if (recievedData.StartsWith(newConfigHeader))
            {
                recievedData = recievedData.Substring(newConfigHeader.Length);
                if (nodeConfiguration.SetNewConfiguration(recievedData))
                {
                    serialPort.WriteLine(deviceConfiguredCommand);
                }
                return;
            }

            // If device model is requested
            if (recievedData == getDeviceModelQuery)
            {
                serialPort.WriteLine($"deviceModel:{nodeConfiguration.DeviceModel}");
                return;
            }

            // If device reset is commanded, reset device.
            if (recievedData == resetNodeCommand)
            {
                nodeConfiguration.ResetNodeToFactory();
                return;
            }


            // No match for configuration commands, so
            // pass data back to delegate (if one is set)
            if (dataRecivedCallbackHandler != null)
            {
                dataRecivedCallbackHandler(recievedData);
                return;
            }
        }

        /// <summary>
        /// Sends a message over the serialport, to any device listening
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessage(string message)
        {
            // Safety checks
            if (!serialPort.IsOpen)
                return false;

            if (message == null)
                return false;

            if (message == string.Empty)
                return false;

            // Transmit data
            try
            {
                serialPort.WriteLine(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}