using System;
using System.IO.Ports;
using System.Device.Gpio;
using Iot.Device.Button;

using SmartDeviceFirmware;
using System.Threading;
using System.Net.WebSockets;
using nanoFramework.Json;
using System.Collections;
using System.Diagnostics;

namespace SmartCurtainsFirmware
{
    class SmartCurtains : Device
    {
        // Pin numbering 
        //
        const int RxUART2PinNumber = 16;
        const int TxUART2PinNumber = 17;
        const int wifiConnectedLedPinNumber = 23;
        const int motorControllerIn1PinNumber = 13;
        const int motorControllerIn2PinNumber = 12;
        const int motorControllerIn3PinNumber = 14;
        const int motorControllerIn4PinNumber = 27;
        const int rollDownButtonPin = 32;
        const int rollUpButtonPin = 33;
        const int CalibrateButtonPin = 25;
        const int StopAllActionButtonPin = 26;

        // ESP32 Configuration
        //
        const int wifiInterfaceIndex = 0;
        const string COMPort = "COM2";

        // Device specific objects
        // 
        MotorController motorController;

        GpioButton rollDownButton;
        GpioButton rollUpButton;
        GpioButton calibrateButton;
        GpioButton stopMotorButton;

        DeviceData requestedDeviceState;
        DeviceData acutalDeviceState;

        /// <summary>
        /// 
        /// </summary>
        public SmartCurtains() : base("Smart Curtains")
        {
            nodeConfiguration.WiFiSSID = "TP-LINK_0AC4EC";
            nodeConfiguration.WiFiPassword = "eqh76rxg";
            nodeConfiguration.DeviceID = new Guid("c7646498-2119-4915-a176-4bacfe5e44c1");
            nodeConfiguration.UserID = new Guid("c7646498-2119-4915-a176-4bacfe5e84c1");

            requestedDeviceState = new DeviceData();
            acutalDeviceState = new DeviceData();

            gpioController = new GpioController(PinNumberingScheme.Board);

            serialCommunicator = new SerialCommunicator(nodeConfiguration, 
                                                        COMPort, 
                                                        RxUART2PinNumber, 
                                                        TxUART2PinNumber, 
                                                        SerialDataRecived);

            wifiHandler = new WiFiHandler(nodeConfiguration, 
                                          wifiInterfaceIndex, 
                                          wifiConnectedLedPinNumber, 
                                          gpioController);
            
            signalRController = new SignalRController(nodeConfiguration, 
                                                      Handle_SetDeviceData, 
                                                      Handle_RequestDeviceData, 
                                                      Handle_DeviceCommand);

            motorController = new MotorController(gpioController,
                                                  motorControllerIn1PinNumber,
                                                  motorControllerIn2PinNumber,
                                                  motorControllerIn3PinNumber,
                                                  motorControllerIn4PinNumber);


            rollDownButton = new GpioButton(rollDownButtonPin, gpioController, false);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController, false);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController, false);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController, false);

            rollDownButton.Press += RollDownButton_Press;
            rollUpButton.Press += RollUpButton_Press;
            stopMotorButton.Press += StopMotorButton_Press;
            calibrateButton.Press += CalibrateButton_Press;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_RequestDeviceData(object sender, object[] args)
        {
            Debug.WriteLine("Data Requested");

            string jsonData = JsonConvert.SerializeObject(requestedDeviceState);
            signalRController.TransmitDeviceData(jsonData);

            // call transmit Device Data, with jsonData of current and REQUESTED device data
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_SetDeviceData(object sender, object[] args)
        {
            Debug.WriteLine("Data Received");
            string jsonData = args[0].ToString();
            Debug.WriteLine(jsonData);

            requestedDeviceState = (DeviceData)JsonConvert.DeserializeObject(jsonData, typeof(DeviceData));

            signalRController.TransmitDeviceAcknowledge();

            // set device data
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_DeviceCommand(object sender, object[] args)
        {
            Console.WriteLine("Command Isued");
            Console.WriteLine(args[0].ToString());
            string command = args[0].ToString();

            // Acknowledge command
            // Perform command

            switch (command)
            {
                case "RollUp":
                    Console.WriteLine($"Command Received: {command}");
                    motorController.SetPoint = motorController.MinSetpoint;
                    break;

                case "RollDown":
                    Console.WriteLine($"Command Received: {command}");
                    motorController.SetPoint = motorController.MaxSetpoint;
                    break;

                default:
                    Console.WriteLine($"Unknown Command Received: {command}");
                    break;
            }
        }


















        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CalibrateButton_Press(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMotorButton_Press(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.CurrentLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollUpButton_Press(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MinSetpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollDownButton_Press(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MaxSetpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recivedData"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SerialDataRecived(string recivedData)
        {
            throw new NotImplementedException();
        }
    }
}
