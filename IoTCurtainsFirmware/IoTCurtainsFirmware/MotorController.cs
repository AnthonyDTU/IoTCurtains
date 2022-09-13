using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace IoTCurtainsFirmware
{
    internal class MotorController
    {
        GpioPin in1;
        GpioPin in2;
        GpioPin in3;
        GpioPin in4;

        private int setPoint = 0;
        private int currentLocation  = 0;
        private int minLocation = 0;
        private int maxLocation = 0;

        private bool calibrated = false;

        public bool Calibrated { get { return calibrated; } set { calibrated = value; } }
        public int CurrentState { get { return (int)((float)(currentLocation / maxLocation) * 100) ; } }
        public int SetPoint { get { return setPoint; } set { setPoint = value; } }
        public int MinSetpoint { get { return minLocation; } set { minLocation = value; } }
        public int MaxSetpoint { get { return maxLocation; } set { maxLocation = value; } }

        public MotorController(GpioController gpioController, 
                               int in1PinNumber, 
                               int in2PinNumber, 
                               int in3PinNumber, 
                               int in4PinNumber, 
                               int motorStepsPerRotation)
        {
            in1 = gpioController.OpenPin(in1PinNumber);
            in2 = gpioController.OpenPin(in2PinNumber);
            in3 = gpioController.OpenPin(in3PinNumber);
            in4 = gpioController.OpenPin(in4PinNumber);
        }


     

        /// <summary>
        /// 
        /// </summary>
        public void runMotor()
        {
            while (true)
            {
                if (currentLocation != setPoint)
                {
                    while (currentLocation != setPoint)
                    {
                        // Check for setpoint limits, and adjust:
                        setPoint = CheckSetpointLimits();


                        Thread.Sleep(1);
                    }
                }

                Thread.Sleep(Timeout.Infinite);
            }
        }

        /// <summary>
        /// Checks the setpoint against its calibrated limits, and if it lies out of bounds, it adjust it to the nearest.
        /// </summary>
        /// <returns>The veryfied setpoint</returns>
        private int CheckSetpointLimits()
        {
            if (setPoint < minLocation) return minLocation;
            if (setPoint > maxLocation) return maxLocation;
            return setPoint;
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
            return false;
        }




        public void RollToButtom()
        {
            if (!calibrated) return;

            setPoint = maxLocation;
        }

        public void RollToTop()
        {
            if (!calibrated) return;
            
            setPoint = minLocation;

            try
            {
                while(currentLocation != setPoint)
                {
                    RollUp();
                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException)
            {
                setPoint = currentLocation;
                return;
            }

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
            setPoint = currentLocation;
        }
    }
}
