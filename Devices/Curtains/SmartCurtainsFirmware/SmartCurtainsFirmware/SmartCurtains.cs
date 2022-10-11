using System;
using System.IO.Ports;
using System.Device.Gpio;
using Iot.Device.Button;

using SmartDeviceFirmware;
namespace SmartCurtainsFirmware
{
    class SmartCurtains : Device
    {

        // Pin numbering :
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

        // Project specific objects
        // 
        MotorController motorController;

        GpioButton rollDownButton;
        GpioButton rollUpButton;
        GpioButton calibrateButton;
        GpioButton stopMotorButton;

        /// <summary>
        /// 
        /// </summary>
        public SmartCurtains()
        {
            gpioController = new GpioController(PinNumberingScheme.Board);

            serialCommunicator = new SerialCommunicator(nodeConfiguration, 
                                                        COMPort, 
                                                        RxUART2PinNumber, 
                                                        TxUART2PinNumber, 
                                                        SerialDataRecived);

            wifiHandler = new WiFiHandler(nodeConfiguration.WiFiSSID, 
                                          nodeConfiguration.WiFiPassword, 
                                          wifiInterfaceIndex, 
                                          wifiConnectedLedPinNumber, 
                                          gpioController);

            motorController = new MotorController(gpioController,
                                                  motorControllerIn1PinNumber,
                                                  motorControllerIn2PinNumber,
                                                  motorControllerIn3PinNumber,
                                                  motorControllerIn4PinNumber);


            rollDownButton = new GpioButton(rollDownButtonPin, gpioController, false);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController, false);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController, false);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController, false);

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


        /// <summary>
        /// Sets the motors setpoint to the current location, to stop its operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMotorButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.CurrentLocation;
        }

        /// <summary>
        /// Sets the motor setpoint to the maximum calibrated value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollToButtomButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MaxSetpoint;
        }

        /// <summary>
        /// Sets the motor setpoint to the minimum calibrated value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RollToTopButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MinSetpoint;
        }
    }
}
