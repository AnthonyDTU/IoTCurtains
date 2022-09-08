using Iot.Device.RotaryEncoder;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Pwm;


namespace IoTCurtainsFirmware
{
    public class Program
    {
        static int pwmPinNumber = 1;
        static int clockwiseRotationPinNumber = 2;

        static int rotaryEncoderPinA = 3;
        static int rotaryEncoderPinB = 4;
        static int rotaryEncoderClicksPerRotation = 10;

        public static void Main()
        {
            GpioController gpioController = new GpioController(PinNumberingScheme.Board);
            QuadratureRotaryEncoder rotaryEncoder = new QuadratureRotaryEncoder(rotaryEncoderPinA, rotaryEncoderPinB, rotaryEncoderClicksPerRotation);

            MotorController motorController = new MotorController(gpioController.OpenPin(pwmPinNumber), gpioController.OpenPin(clockwiseRotationPinNumber), rotaryEncoder);

            while (true)
            {
                

            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
