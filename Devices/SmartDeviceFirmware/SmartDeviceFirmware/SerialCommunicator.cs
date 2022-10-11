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

        private const string getConfigCommand = "getConfig";
        private const string setConfigCommand = "setConfig";
        private const string resetNodeCommand = "resetNode";
        private const string readyForConfigCommand = "readyForConfig";


        public delegate void DataRecivedHandler(string data);
        public DataRecivedHandler dataRecivedCallbackHandler;

        private NodeConfiguration nodeConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="COMPort"></param>
        /// <param name="rxPinNumber"></param>
        /// <param name="txPinNumber"></param>
        /// <param name="dataRecivedHandler">Passes through all data, which are not related to config commands</param>
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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string recievedData = serialPort.ReadLine();

            // If data recived is not a config command, pass data to callback method (if any is set)
            if (recievedData != getConfigCommand &&
                recievedData != setConfigCommand &&
                recievedData != resetNodeCommand)
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
                serialPort.WriteLine(readyForConfigCommand);
                while (serialPort.BytesToRead == 0) ;
                string newConfigJson = serialPort.ReadLine();
                nodeConfiguration.SetNewConfiguration(newConfigJson);
                return;
            }

            // If device reset is commanded, reset device.
            if (recievedData == resetNodeCommand)
            {
                nodeConfiguration.ResetNodeToFactory();
                return;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessage(string message)
        {
            if (!serialPort.IsOpen)
                return false;

            if (message == null)
                return false;

            if (message == string.Empty)
                return false;

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
