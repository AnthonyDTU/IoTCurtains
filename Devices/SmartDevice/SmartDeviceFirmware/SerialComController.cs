using System;
using System.IO.Ports;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;

namespace SmartDeviceFirmware
{
    public static class SerialComController
    {
        private static SerialPort serialPort;
        private static int baudRate = 115200;
        private static Parity parity = Parity.None;
        private static int databits = 8;
        private static StopBits stopBits = StopBits.One;

        private static string getConfigCommand = "deviceConfig?";
        private static string newConfigHeader = "newConfig:";
               
        private static string getDeviceModelQuery = "deviceModel?";
        private static string readyForConfigQuery = "readyForConfig?";
        private static string readyForConfigResponse = "readyForConfig!";
        private static string deviceConfiguredCommand = "deviceConfigured!";
        private static string resetNodeCommand = "resetNode";
        
        public delegate void DataRecivedHandler(string data);
        public static DataRecivedHandler dataRecivedCallback;

        /// <summary>
        /// Configures the SerialComController with the values from an implementing device.
        /// Opens a serial port and starts listening for data.
        /// </summary>
        /// <param name="COMPort"></param>
        /// <param name="rxPinNumber"></param>
        /// <param name="txPinNumber"></param>
        /// <param name="dataRecivedCallbackHandler"></param>
        public static void Configure(string COMPort, int rxPinNumber, int txPinNumber, DataRecivedHandler dataRecivedCallbackHandler = null)
        {
            Configuration.SetPinFunction(rxPinNumber, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(txPinNumber, DeviceFunction.COM2_TX);
            dataRecivedCallback = dataRecivedCallbackHandler;

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
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
                if (NodeConfiguration.SetNewConfiguration(recievedData))
                {
                    serialPort.WriteLine(deviceConfiguredCommand);
                }
                return;
            }

            // If device model is requested
            if (recievedData == getDeviceModelQuery)
            {
                serialPort.WriteLine($"deviceModel:{NodeConfiguration.DeviceModel}");
                return;
            }

            // If device reset is commanded, reset device.
            if (recievedData == resetNodeCommand)
            {
                NodeConfiguration.ResetNodeToFactory();
                return;
            }


            // No match for configuration commands, so
            // pass data back to delegate (if one is set)
            if (dataRecivedCallback != null)
            {
                dataRecivedCallback(recievedData);
                return;
            }
        }

        /// <summary>
        /// Sends a message over the serialport, to any device listening
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendMessage(string message)
        {
            // Safety checks
            if (!serialPort.IsOpen)
                return false;

            if (message == null)
                return false;

            if (message == string.Empty)
                return false;

            lock (serialPort)
            {
                // Transmit data
                try
                {
                    serialPort.WriteLine(message);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error transmitting data on the serial port {ex.Message}");
                    return false;
                }
            }

            
        }
    }
}
