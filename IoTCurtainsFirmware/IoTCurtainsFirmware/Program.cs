using System;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Button;
using System.Device.Wifi;
using System.Net.NetworkInformation;
using nanoFramework.Networking;

using nanoFramework.Azure;
using nanoFramework.Azure.Devices.Client;
using System.Security.Cryptography.X509Certificates;
using System.IO.Ports;
using System.Collections;
using nanoFramework.Json;
using nanoFramework.Hardware.Esp32;
using DeviceConfiguration;

namespace IoTCurtainsFirmware
{
    public class Program
    {
        // Move to secrets, and possible make configurable from controller app.

        static NodeConfiguration deviceConfiguration = new NodeConfiguration();

        static int motorStepsPerRotation = 2038;

        static int rollDownButtonPin = 32;
        static int rollUpButtonPin = 33;
        static int rollToButtomButtonPin = 22;
        static int rollToTopButtonPin = 23;

        static int StopAllActionButtonPin = 21;

        static int CalibrateButtonPin = 36;

        static int RxUART2PinNumber = 16;
        static int TxUART2PinNumber = 17;

        static int motorControllerIn1PinNumber = 13;
        static int motorControllerIn2PinNumber = 12;
        static int motorControllerIn3PinNumber = 14;
        static int motorControllerIn4PinNumber = 27;
        static int wifiConnectedLedPinNumber = 19;

        static GpioController gpioController;
        static MotorController motorController;

        static GpioButton rollDownButton;
        static GpioButton rollUpButton;
        static GpioButton rollToButtomButton;
        static GpioButton rollToTopButton;
        static GpioButton calibrateButton;
        static GpioButton stopMotorButton;

        static GpioPin wifiConnectedLedIndicator;
        private static int networkInterfaceIndex = 0;
        static bool connectedToWiFi = false;

        static SerialPort serialPort;

        public static void Main()
        {
            InitializeSystem();
            //CalibrateMotorController();




            //const string DeviceID = "CurtainController";
            //const string IotBrokerAddress = "IoTCurtains.azure-devices.net";
            //const string SasKey = "HvCMwRqzTDAjCkQZvDvEu/ziH6nX1RP41QTpmYv2h3o=";
            //DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey);
            //bool connectedToAzure = azureIoT.Open();

            //if (connectedToAzure == true)
            //{
            //    Console.WriteLine("Successfully connected to Azure!");
            //}

            Configuration.SetPinFunction(RxUART2PinNumber, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(TxUART2PinNumber, DeviceFunction.COM2_TX);

            var ports = SerialPort.GetPortNames();
            foreach(var port in ports)
            {
                Console.WriteLine(port);
            }

            serialPort = new SerialPort(portName: "COM2",
                                        baudRate: 115200,
                                        Parity.None,
                                        dataBits: 8,
                                        StopBits.One);

            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();

            // Program life 
            while (true)
            {
                if (!connectedToWiFi)
                {
                    ConnectToWiFi();
                }

                while (rollDownButton.IsPressed)
                {
                    motorController.SetPoint++;
                    Thread.Sleep(2);
                }
                
                while (rollUpButton.IsPressed)
                {
                    motorController.SetPoint--;
                    Thread.Sleep(2);
                }
            }

            Thread.Sleep(Timeout.Infinite);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string command = serialPort.ReadLine();

            switch (command)
            {
                case "getConfig":
                    serialPort.WriteLine(JsonConvert.SerializeObject(deviceConfiguration);
                    break;

                case "configure":
                    serialPort.WriteLine("Send Config");
                    while (serialPort.BytesToRead == 0) ;
                    string newConfigJson = serialPort.ReadLine();
                    deviceConfiguration = (NodeConfiguration)JsonConvert.DeserializeObject(newConfigJson, typeof(NodeConfiguration));
                    break;

                case "resetDevice":
                    deviceConfiguration = new NodeConfiguration();
                    serialPort.WriteLine("Device Reset!");
                    break;

                default:
                    serialPort.WriteLine("Error - Unkown Command!");
                    break;
            }
        }


        /// <summary>
        /// Initilizes the following System Components:
        /// 
        /// - GPIO Controller
        /// - MotorController
        /// - Input Buttons
        /// - LED's
        /// - WiFi
        /// - UART
        /// 
        /// </summary>
        private static void InitializeSystem()
        {
            // GPIO Contoller:
            gpioController = new GpioController(PinNumberingScheme.Board);

            // Motor Controller:
            motorController = new MotorController(gpioController,
                                                  motorControllerIn1PinNumber,
                                                  motorControllerIn2PinNumber,
                                                  motorControllerIn3PinNumber,
                                                  motorControllerIn4PinNumber);

            // Buttons
            rollDownButton = new GpioButton(rollDownButtonPin, gpioController, false);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController, false);
            rollToButtomButton = new GpioButton(rollToButtomButtonPin, gpioController, false);
            rollToTopButton = new GpioButton(rollToTopButtonPin, gpioController, false);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController, false);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController, false);
                        
            rollToTopButton.ButtonDown += RollToTopButton_ButtonDown;
            rollToButtomButton.ButtonDown += RollToButtomButton_ButtonDown;
            stopMotorButton.ButtonDown += StopMotorButton_ButtonDown;

            // WiFi
            ConnectToWiFi();
        }

        /// <summary>
        /// Handles connection to WiFi
        /// </summary>
        private static void ConnectToWiFi()
        {
            if (WifiNetworkHelper.ConnectDhcp(deviceConfiguration.WiFiSSID,
                                              deviceConfiguration.WiFiPassword,
                                              WifiReconnectionKind.Automatic,
                                              requiresDateTime: true,
                                              wifiAdapterId: networkInterfaceIndex,
                                              token: new CancellationTokenSource(10000).Token))
            {
                connectedToWiFi = true;
                
                NetworkInterface network = NetworkInterface.GetAllNetworkInterfaces()[networkInterfaceIndex];
                Console.WriteLine("Connected To Wifi!");
                Console.WriteLine($"Network IP: {network.IPv4Address}");

                wifiConnectedLedIndicator = gpioController.OpenPin(wifiConnectedLedPinNumber, PinMode.Output);
                wifiConnectedLedIndicator.Write(PinValue.High);
            }
            else
            {
                Console.WriteLine("Could Not Connect To Wifi!");
            }
        }


        /// <summary>
        /// Performs the calibration of the stepper motors position on power-up
        /// The sequence of calibration is as follows:
        /// 
        /// User rolls to top.
        /// User declare where the top is.
        /// User rolls to the buttom
        /// User declares buttom is reached.
        /// 
        /// </summary>
        private static void CalibrateMotorController()
        {
            bool topCalibrated = false;
            while (!motorController.Calibrated)
            {
                while (rollDownButton.IsPressed)
                {
                    motorController.SetPoint++;
                    Thread.Sleep(2);
                }

                while (rollUpButton.IsPressed)
                {
                    motorController.SetPoint--;
                    Thread.Sleep(2);
                }

                if (calibrateButton.IsPressed)
                {
                    while (calibrateButton.IsPressed) ;
                    if (!topCalibrated)
                    {
                        motorController.MinSetpoint = motorController.SetPoint;
                        topCalibrated = true;
                        continue;
                    }
                    else
                    {
                        motorController.MaxSetpoint = motorController.SetPoint;
                        motorController.Calibrated = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the motors setpoint to the current location, to stop its operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void StopMotorButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.CurrentLocation;
        }

        /// <summary>
        /// Sets the motor setpoint to the maximum calibrated value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RollToButtomButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MaxSetpoint;
        }

        /// <summary>
        /// Sets the motor setpoint to the minimum calibrated value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RollToTopButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MinSetpoint;
        }
    }
}







