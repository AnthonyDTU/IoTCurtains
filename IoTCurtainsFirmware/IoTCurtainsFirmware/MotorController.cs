using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.DCMotor;
using Iot.Device.RotaryEncoder;

namespace IoTCurtainsFirmware
{
    internal class MotorController
    {
        GpioPin pwmPin;
        GpioPin clockwiseDirectionPin;
        QuadratureRotaryEncoder rotaryEncoder;

        private int setPoint = 0;
        private int currentLocation  = 0;
        private int minLocation = 0;
        private int maxLocation = 200;

        private bool calibrated = false;


        public int CurrentState { get { return (int)((float)(currentLocation / maxLocation) * 100) ; } }
        public int SetPoint { get { return setPoint ; } set { setPoint = SetPoint; } }
        
        public MotorController(GpioPin pwmPin, GpioPin clockwiseDirectionPin, QuadratureRotaryEncoder rotaryEncoder)
        {
            this.pwmPin = pwmPin;
            this.clockwiseDirectionPin = clockwiseDirectionPin;
            this.rotaryEncoder = rotaryEncoder;
        }


      










    }
}
