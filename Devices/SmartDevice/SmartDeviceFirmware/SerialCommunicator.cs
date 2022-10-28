using System;
using System.IO.Ports;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;

namespace SmartDeviceFirmware
{
    public class SerialCommunicator
    {
        SerialPort serialPort;
        int baudRate = 115200;
        Parity parity = Parity.None;
        int databits = 8;
        StopBits stopBits = StopBits.One;

        private const string getConfigCommand = "deviceConfig?";
        private const string setConfigCommand = "config";
        private const string resetNodeCommand = "resetNode";
        private const string getDeviceModelCommand = "deviceModel?";
        private const string readyForConfigResponse = "readyForConfig";


        public delegate void DataRecivedHandler(string data);
        public DataRecivedHandler dataRecivedCallbackHandler;

        private NodeConfiguration nodeConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="COMPort"></param>
        /// <param name="rxPinNumber"></param>
        /// <param name="txPinNumber"></param>
        /// <param name="dataRecivedHandler">Passes back all data, that is not related to config commands</param>
        public SerialCommunicator(NodeConfiguration nodeConfiguration, string COMPort, int rxPinNumber, int txPinNumber, DataRecivedHandler dataRecivedCallbackHandler = null)
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

            // If data recived is not a config command, pass data to callback method (if any is set)
            if (recievedData != getConfigCommand &&
                recievedData != setConfigCommand &&
                recievedData != resetNodeCommand &&
                recievedData != getDeviceModelCommand)
            {
                if (dataRecivedCallbackHandler != null)
                {
                    dataRecivedCallbackHandler(recievedData);
                }
                return;
            }

            // If config is requested, transmit it to the requester
            if (recievedData == getConfigCommand)
            {
                serialPort.WriteLine(JsonConvert.SerializeObject(nodeConfiguration));
                return;
            }

            // If new configuration is incoming, parse and store it
            if (recievedData == setConfigCommand)
            {
                serialPort.WriteLine(readyForConfigResponse);
                while (serialPort.BytesToRead == 0) ;
                string newConfigJson = serialPort.ReadLine();
                nodeConfiguration.SetNewConfiguration(newConfigJson);
                return;
            }

            // If device model is requested
            if (recievedData == getDeviceModelCommand)
            {
                serialPort.WriteLine(nodeConfiguration.DeviceModel);
            }

            // If device reset is commanded, reset device.
            if (recievedData == resetNodeCommand)
            {
                nodeConfiguration.ResetNodeToFactory();
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
