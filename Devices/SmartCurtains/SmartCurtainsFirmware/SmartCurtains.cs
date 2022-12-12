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
using Windows.Storage;
using nanoFramework.Runtime.Native;

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
        const int activateMotorPinNumber = 18;
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
        private MotorController motorController;

        private GpioButton rollDownButton;
        private GpioButton rollUpButton;
        private GpioButton calibrateButton;
        private GpioButton stopMotorButton;

        private DeviceData requestedDeviceState;
        private DeviceData actualDeviceState;

        /// <summary>
        /// Constructor
        /// 
        /// Initializes the Device class, and configures the utillity helper classes in the SmartDeviceFirmware.dll
        /// </summary>
        public SmartCurtains() : base("Smart Curtains")
        {
            requestedDeviceState = new DeviceData();
            actualDeviceState = new DeviceData();

            gpioController = new GpioController(PinNumberingScheme.Board);

            SerialComController.Configure(COMPort,
                                         RxUART2PinNumber,
                                         TxUART2PinNumber,
                                         SerialDataReceived);

            WiFiController.Configure(wifiInterfaceIndex,
                                     wifiConnectedLedPinNumber,
                                     gpioController);

            SignalRController.Configure(Handle_SetDeviceData,
                                        Handle_RequestDeviceData,
                                        Handle_DeviceCommand);

            motorController = new MotorController(gpioController,
                                                  actualDeviceState,
                                                  Handle_ReportData,
                                                  motorControllerIn1PinNumber,
                                                  motorControllerIn2PinNumber,
                                                  motorControllerIn3PinNumber,
                                                  motorControllerIn4PinNumber,
                                                  activateMotorPinNumber);

            rollDownButton = new GpioButton(rollDownButtonPin, gpioController, false);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController, false);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController, false);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController, false);

            rollDownButton.ButtonDown += RollDownButton_Press;
            rollUpButton.ButtonDown += RollUpButton_Press;
            stopMotorButton.ButtonDown += StopMotorButton_Press;
            calibrateButton.ButtonDown += CalibrateButton_Press;
        }


        /// <summary>
        /// Handler for the device data is requested
        /// Passed to SignalRController as a callback delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_RequestDeviceData(object sender, object[] args)
        {
            Debug.WriteLine("Data Requested");

            string jsonData = JsonConvert.SerializeObject(actualDeviceState);
            SignalRController.TransmitDeviceData(jsonData);
        }



        /// <summary>
        /// Handler for when new data is transmitted to the device
        /// Passed to SignalRController as a callback delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_SetDeviceData(object sender, object[] args)
        {
            Debug.WriteLine("Data Received");
            string jsonData = args[0].ToString();
            Debug.WriteLine(jsonData);

            requestedDeviceState = (DeviceData)JsonConvert.DeserializeObject(jsonData, typeof(DeviceData));

            actualDeviceState.RollDownTime = requestedDeviceState.RollDownTime;
            actualDeviceState.RollUpTime = requestedDeviceState.RollUpTime;
            actualDeviceState.FollowSunset = requestedDeviceState.FollowSunset;
            actualDeviceState.FollowSunrise = requestedDeviceState.FollowSunrise;

            SignalRController.TransmitDeviceAcknowledge();
        }

        /// <summary>
        /// Handler for when a command is received from the platform
        /// Passed to SignalRController as a callback delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void Handle_DeviceCommand(object sender, object[] args)
        {
            Console.WriteLine("Command Isued");
            Console.WriteLine(args[0].ToString());
            string command = args[0].ToString();

            // Acknowledge command
            SignalRController.TransmitDeviceAcknowledge();

            // Perform command
            switch (command)
            {
                case "\"RollUp\"":
                    Console.WriteLine($"Command Received: {command}");
                    motorController.SetPoint = motorController.MinSetpoint;
                    break;

                case "\"RollDown\"":
                    Console.WriteLine($"Command Received: {command}");
                    motorController.SetPoint = motorController.MaxSetpoint;
                    break;

                case "\"DeleteDevice\"":
                    Console.WriteLine("DeviceDeleted");
                    NodeConfiguration.ResetNodeToFactory();
                    break;

                default:
                    Console.WriteLine($"Unknown Command Received: {command}");
                    break;
            }
        }

        /// <summary>
        /// Handler for reporting data to the platform
        /// Passed to MotorController as a callback delegate
        /// </summary>
        internal void Handle_ReportData()
        {
            string jsonData = JsonConvert.SerializeObject(actualDeviceState);
            SignalRController.TransmitDeviceData(jsonData);
        }


        /// <summary>
        /// Handler for when the calibration button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CalibrateButton_Press(object sender, EventArgs e)
        {
            Debug.WriteLine("Calibrate Button Pressed");
            // Calibrate motor
        }

        /// <summary>
        /// Handler for when the stop button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMotorButton_Press(object sender, EventArgs e)
        {
            Debug.WriteLine("Stop Button Pressed");
            motorController.SetPoint = motorController.CurrentLocation;
        }

        /// <summary>
        /// Handler for when the Roll Up button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollUpButton_Press(object sender, EventArgs e)
        {
            Debug.WriteLine("Roll Up Button Pressed");
            motorController.SetPoint = motorController.MinSetpoint;
        }

        /// <summary>
        /// Handler for when the Roll Down Button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollDownButton_Press(object sender, EventArgs e)
        {
            Debug.WriteLine("Roll Down Button Pressed");
            motorController.SetPoint = motorController.MaxSetpoint;
        }

        /// <summary>
        /// Handler for when serial data is received. 
        /// Passed to SerialComController as a callback delegate
        /// </summary>
        /// <param name="recivedData"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SerialDataReceived(string recivedData)
        {
            Console.WriteLine(recivedData);
        }
    }
}
