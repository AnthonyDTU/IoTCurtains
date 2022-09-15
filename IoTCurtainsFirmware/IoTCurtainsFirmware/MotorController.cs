using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace IoTCurtainsFirmware
{
    internal class MotorController
    {
        //static int[] stepSequence0 = { 1, 1, 0, 0 };
        //static int[] stepSequence1 = { 0, 1, 1, 0 };
        //static int[] stepSequence2 = { 0, 0, 1, 1 };
        //static int[] stepSequence3 = { 1, 0, 0, 1 };

        //static int[] stepSequence0 = { 0, 0, 0, 0, 0, 1, 1, 1 };
        //static int[] stepSequence1 = { 0, 0, 0, 1, 1, 1, 0, 0 };
        //static int[] stepSequence2 = { 0, 1, 1, 1, 0, 0, 0, 0 };
        //static int[] stepSequence3 = { 1, 1, 0, 0, 0, 0, 0, 1 };


        int[] stepSequence0 = { 0, 0, 1, 1 };
        int[] stepSequence1 = { 1, 0, 0, 1 };
        int[] stepSequence2 = { 1, 1, 0, 0 };
        int[] stepSequence3 = { 0, 1, 1, 0 };

        //static int[,] stepSequence =
        //{
        //    { 1, 0, 1, 0 },
        //    { 0, 1, 1, 0 },
        //    { 0, 1, 0, 1 },
        //    { 1, 0, 0, 1 }
        //};

        public AutoResetEvent resetEvent = new AutoResetEvent(false);

        private GpioPin in1;
        private GpioPin in2;
        private GpioPin in3;
        private GpioPin in4;

        private int currentStep = 0;
        private int numberOfSteps = 4;

        private int setPoint = 1700;
        private int currentLocation  = 1750;
        
        private int minLocation = 0;
        private int maxLocation = 2000;

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
                               int in4PinNumber)
        {
            in1 = gpioController.OpenPin(in1PinNumber, PinMode.Output);
            in2 = gpioController.OpenPin(in2PinNumber, PinMode.Output);
            in3 = gpioController.OpenPin(in3PinNumber, PinMode.Output);
            in4 = gpioController.OpenPin(in4PinNumber, PinMode.Output);
        }




        /// <summary>
        /// 
        /// </summary>
        [MTAThread]
        public void runMotor()
        {
            while (true)
            {
                // Check for setpoint limits, and adjust:
                setPoint = CheckSetpointLimits();

                while (currentLocation != setPoint)
                {
                    if (currentLocation < setPoint)
                    {
                        currentLocation++;
                        currentStep = (currentStep == numberOfSteps - 1) ? 0 : currentStep + 1;
                    }
                    else if (currentLocation > setPoint)
                    {
                        currentLocation--;
                        currentStep = (currentStep == 0) ? numberOfSteps - 1 : currentStep - 1;
                    }

                    in1.Write(stepSequence0[currentStep]);
                    in2.Write(stepSequence1[currentStep]);
                    in3.Write(stepSequence2[currentStep]);
                    in4.Write(stepSequence3[currentStep]);
                    Thread.Sleep(5);
                }

                resetEvent.WaitOne();
                resetEvent.Reset();

                //Thread.Sleep(Timeout.Infinite);
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
