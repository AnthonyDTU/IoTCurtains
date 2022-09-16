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


        //private readonly int[] stepSequence0 = { 0, 0, 1, 1 };
        //private readonly int[] stepSequence1 = { 1, 0, 0, 1 };
        //private readonly int[] stepSequence2 = { 1, 1, 0, 0 };
        //private readonly int[] stepSequence3 = { 0, 1, 1, 0 };

        private readonly int[] stepSequence0 = { 1, 0, 0, 0 };
        private readonly int[] stepSequence1 = { 0, 1, 0, 0 };
        private readonly int[] stepSequence2 = { 0, 0, 1, 0 };
        private readonly int[] stepSequence3 = { 0, 0, 0, 1 };

        private AutoResetEvent resetEvent = new AutoResetEvent(false);

        private GpioPin in1;
        private GpioPin in2;
        private GpioPin in3;
        private GpioPin in4;

        private int currentStep = 0;
        private readonly int numberOfSteps = 4;

        private int setPoint = 1700;
        private int currentLocation  = 1750;
        
        private int minLocation = 0;
        private int maxLocation = 2000;

        private bool calibrated = false;

        private Thread engineThread;

        public bool Calibrated 
        { 
            get { return calibrated; } 
            set { calibrated = value; } 
        }
        public int CurrentState 
        { 
            get { return (int)((float)(currentLocation / maxLocation) * 100) ; } 
        }
        public int SetPoint 
        {
            get { return setPoint; } 
            set { setPoint = CheckSetpointLimits(value); } 
        }
        public int MinSetpoint 
        { 
            get { return minLocation; } 
            set { minLocation = value; } 
        }
        public int MaxSetpoint 
        { 
            get { return maxLocation; } 
            set { maxLocation = value; } 
        }

        public AutoResetEvent ResetEvent 
        { 
            get { return resetEvent; } 
        } 

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

            engineThread = new Thread(new ThreadStart(this.RunMotor));
            engineThread.Priority = ThreadPriority.AboveNormal;
            engineThread.Start();
        }


        /// <summary>
        /// Checks the setpoint against its calibrated limits, and if it lies out of bounds, it adjust it to the nearest.
        /// </summary>
        /// <returns>The veryfied setpoint</returns>
        private int CheckSetpointLimits(int newSetpoint)
        {
            if (newSetpoint < minLocation) return minLocation;
            if (newSetpoint > maxLocation) return maxLocation;
            return newSetpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunMotor()
        {
            while (true)
            {
                // Check for setpoint limits, and adjust:
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
                    Thread.Sleep(2);
                }

                resetEvent.WaitOne();
                resetEvent.Reset(); // ???
            }
        }
    }
}
