using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.DCMotor;
using Iot.Device.RotaryEncoder;
using System.Device.Pwm;
using Windows.Devices.Pwm;

namespace IoTCurtainsFirmware
{
    internal class MotorController
    {
        GpioPin clockwiseDirectionPin;
        QuadratureRotaryEncoder rotaryEncoder;
        PwmChannel pwmChannel;

        private int setPoint = 0;
        private int currentLocation  = 0;
        private int minLocation = 0;
        private int maxLocation = 200;

        private bool calibrated = false;


        public int CurrentState { get { return (int)((float)(currentLocation / maxLocation) * 100) ; } }
        public int SetPoint { get { return setPoint ; } set { setPoint = SetPoint; } }

        public MotorController(GpioController gpioController, 
                               int pwmPinNumber, 
                               int clockWiseDirectionPinNumber, 
                               int rotaryEncoderPinA, 
                               int rotaryEncoderPinB, 
                               int rotaryEncoderCountsPerRotation)
        {
            pwmChannel = PwmChannel.CreateFromPin(pwmPinNumber);
            clockwiseDirectionPin = gpioController.OpenPin(clockWiseDirectionPinNumber);
            rotaryEncoder = new QuadratureRotaryEncoder(rotaryEncoderPinA, rotaryEncoderPinB, rotaryEncoderCountsPerRotation);
        }


      










    }
}
