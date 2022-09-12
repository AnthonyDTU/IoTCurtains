using Iot.Device.RotaryEncoder;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;


namespace IoTCurtainsFirmware
{
    public class Program
    {
        static int pwmPinNumber = 23;
        static int clockwiseRotationPinNumber = 36;

        static int rotaryEncoderPinA = 39;
        static int rotaryEncoderPinB = 34;
        static int rotaryEncoderClicksPerRotation = 2038;

        static int rollDownButtonPin = 32;
        static int rollUpButtonPin = 33;
        static int rollToButtomButtonPin = 25;
        static int rollToTopButtonPin = 26;

        static int StopAllActionButtonPin = 27;

        static int CalibrateButtonPin = 10;

        static int rollDownLEDIndicatorPinNumber = 19;
        static int rollUpLEDInticatiorPinNumber = 18;

        static MotorController motorController;

        static GpioPin upLED;
        static GpioPin downLED;

        public static void Main()
        {
            // Initialize
            GpioController gpioController = new GpioController(PinNumberingScheme.Board);
           
            motorController = new MotorController(gpioController, 
                                                  pwmPinNumber, 
                                                  clockwiseRotationPinNumber, 
                                                  rotaryEncoderPinA,
                                                  rotaryEncoderPinB, 
                                                  rotaryEncoderClicksPerRotation);
            
            GpioButton rollDownButton = new GpioButton(rollDownButtonPin, gpioController);
            GpioButton rollUpButton = new GpioButton(rollUpButtonPin, gpioController);
            GpioButton rollToButtomButton = new GpioButton(rollToButtomButtonPin, gpioController);
            GpioButton rollToTopButton = new GpioButton(rollToTopButtonPin, gpioController);
            GpioButton stopActionButton = new GpioButton(StopAllActionButtonPin, gpioController);
            GpioButton calibrateButton = new GpioButton(CalibrateButtonPin, gpioController);

            upLED = gpioController.OpenPin(rollUpLEDInticatiorPinNumber, PinMode.Output);
            downLED = gpioController.OpenPin(rollDownLEDIndicatorPinNumber, PinMode.Output);


            rollDownButton.Press += RollDownButton_Press;

            rollUpButton.ButtonDown += RollUpButton_ButtonDown;
            rollUpButton.ButtonUp += RollUpButton_ButtonUp;


            // Calibration:
            bool topCalibrated = false;
            while (!motorController.Calibrated)
            {
                while (rollDownButton.IsPressed)
                {
                    motorController.RollDown();
                    Thread.Sleep(5);
                }

                while (rollUpButton.IsPressed)
                {
                    motorController.RollUp();
                    Thread.Sleep(5);
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



            // Program life 
            while (true)
            {
                while (rollDownButton.IsPressed)
                {
                    motorController.RollDown();
                    Thread.Sleep(5);
                }

                while (rollUpButton.IsPressed)
                {
                    motorController.RollUp();
                    Thread.Sleep(5);
                }
                downLED.Write(PinValue.Low);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        private static void RollUpButton_ButtonUp(object sender, EventArgs e)
        {
            upLED.Write(PinValue.Low);
        }

        private static void RollUpButton_ButtonDown(object sender, EventArgs e)
        {
            upLED.Write(PinValue.High);
        }

        private static void RollDownButton_Press(object sender, EventArgs e)
        {
            downLED.Toggle();
        }
    }
}
