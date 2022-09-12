using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.DCMotor;
using Iot.Device.RotaryEncoder;
using System.Device.Pwm;

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


        public bool Calibrated { get { return calibrated; } set { calibrated = value; } }
        public int CurrentState { get { return (int)((float)(currentLocation / maxLocation) * 100) ; } }
        public int SetPoint { get { return setPoint; } set { setPoint = value; } }
        public int MinSetpoint { set { minLocation = value; } }
        public int MaxSetpoint { set { maxLocation = value; } }

        public MotorController(GpioController gpioController, 
                               int pwmPinNumber, 
                               int clockWiseDirectionPinNumber, 
                               int rotaryEncoderPinA, 
                               int rotaryEncoderPinB, 
                               int rotaryEncoderCountsPerRotation)
        {
            pwmChannel = PwmChannel.CreateFromPin(pwmPinNumber);
            clockwiseDirectionPin = gpioController.OpenPin(clockWiseDirectionPinNumber);
            rotaryEncoder = new QuadratureRotaryEncoder(rotaryEncoderPinA, rotaryEncoderPinB, PinEventTypes.Rising, rotaryEncoderCountsPerRotation, gpioController, false);

            if (Calibrate())
            {
                calibrated = true;
            }
            else
            {
                // Handle Calibration Error
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
        /// <returns></returns>
        private bool Calibrate()
        {
            bool topCalibrated = false;
            bool buttomCalibrated = false;
            while (!topCalibrated)
            {
                wh
            }


            return false;
        }




        public bool RollToButtom()
        {
            if (!calibrated) return false;

            setPoint = maxLocation;
            return true;
        }

        public bool RollToTop()
        {
            if (!calibrated) return false;

            setPoint = minLocation;
            return true;
        }

        /// <summary>
        /// Turns the stepper motor one step down
        /// </summary>
        /// <returns></returns>
        public bool RollDown()
        {
            if (!calibrated) return false;

            setPoint++;
            return true;
        }

        /// <summary>
        /// Turns the stepper motor one step up
        /// </summary>
        /// <returns></returns>
        public bool RollUp()
        {
            if (!calibrated) return false;

            setPoint--;
            return true;
        }

        public void Stop()
        {
            pwmChannel.Stop();
            setPoint = currentLocation;
        }
    }
}
