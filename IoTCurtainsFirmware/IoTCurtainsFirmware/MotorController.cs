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

        private int currentState;
        private int clicksFromMin;
        private int clicksFromMax;

        private bool calibrated = false;
        
        public MotorController(GpioPin pwmPin, GpioPin clockwiseDirectionPin, QuadratureRotaryEncoder rotaryEncoder)
        {
            this.pwmPin = pwmPin;
            this.clockwiseDirectionPin = clockwiseDirectionPin;
            this.rotaryEncoder = rotaryEncoder;
        }










    }
}
