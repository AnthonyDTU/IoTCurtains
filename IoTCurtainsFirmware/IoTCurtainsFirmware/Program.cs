using Iot.Device.RotaryEncoder;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Pwm;
using Iot.Device.Button;


namespace IoTCurtainsFirmware
{
    public class Program
    {
        static int pwmPinNumber = 1;
        static int clockwiseRotationPinNumber = 2;

        static int rotaryEncoderPinA = 3;
        static int rotaryEncoderPinB = 4;
        static int rotaryEncoderClicksPerRotation = 10;

        static int rollDownButtonPin = 11;
        static int rollUpButtonPin = 12;
        static int rollToButtomButtonPin = 13;
        static int rollToTopButtonPin = 14;

        static int StopAllActionButtonPin = 20;

        public static void Main()
        {

            // Initialize
            GpioController gpioController = new GpioController(PinNumberingScheme.Board);
            QuadratureRotaryEncoder rotaryEncoder = new QuadratureRotaryEncoder(rotaryEncoderPinA, rotaryEncoderPinB, rotaryEncoderClicksPerRotation);

            MotorController motorController = new MotorController(gpioController, 
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

            // Multiple fires? What interval?
            rollDownButton.IsHoldingEnabled = true;
            rollDownButton.Holding += RollDownButton_Holding;
            



            while (true)
            {
                

            }

            Thread.Sleep(Timeout.Infinite);
        }

        private static void RollDownButton_Holding(object sender, ButtonHoldingEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
