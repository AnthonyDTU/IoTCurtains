using System;
using System.Threading;
using System.Device.Gpio;

namespace SmartCurtainsFirmware
{
    class MotorController
    {
        static int[] stepSequence0 = { 1, 1, 0, 0 };
        static int[] stepSequence1 = { 0, 1, 1, 0 };
        static int[] stepSequence2 = { 0, 0, 1, 1 };
        static int[] stepSequence3 = { 1, 0, 0, 1 };

        private AutoResetEvent newSetpointSignal = new AutoResetEvent(false);

        private GpioPin in1;
        private GpioPin in2;
        private GpioPin in3;
        private GpioPin in4;
        private GpioPin activateMotor;

        private int currentStep = 0;
        private readonly int numberOfSteps = 4;

        private int setPoint = 1700;
        private int currentLocation = 1750;

        private int minLocation = 0;
        private int maxLocation = 2048;

        private bool calibrated = false;

        private Thread engineThread;
        private DeviceData deviceData;


        public bool Calibrated
        {
            get { return calibrated; }
            set { calibrated = value; }
        }
        public int CurrentLocation
        {
            get { return currentLocation; }
        }
        public int CurrentLocationPercentage
        {
            get { return (int)((float)((float)currentLocation / (float)maxLocation) * 100); }
        }
        public int SetPoint
        {
            get { return setPoint; }
            set
            {
                setPoint = CheckSetpointLimits(value);
                newSetpointSignal.Set();
            }
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

        public delegate void TransmitDataHandler();
        private TransmitDataHandler transmitDataHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpioController"></param>
        /// <param name="in1PinNumber"></param>
        /// <param name="in2PinNumber"></param>
        /// <param name="in3PinNumber"></param>
        /// <param name="in4PinNumber"></param>
        public MotorController(GpioController gpioController,
                               DeviceData deviceData,
                               TransmitDataHandler transmitDataHandler,
                               int in1PinNumber,
                               int in2PinNumber,
                               int in3PinNumber,
                               int in4PinNumber,
                               int activateMoterPinNumber)
        {
            this.deviceData = deviceData;
            this.transmitDataHandler = transmitDataHandler;

            in1 = gpioController.OpenPin(in1PinNumber, PinMode.Output);
            in2 = gpioController.OpenPin(in2PinNumber, PinMode.Output);
            in3 = gpioController.OpenPin(in3PinNumber, PinMode.Output);
            in4 = gpioController.OpenPin(in4PinNumber, PinMode.Output);
            activateMotor = gpioController.OpenPin(activateMoterPinNumber, PinMode.Output);
            
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
        /// This method is run exclusively by the engineThread. 
        /// 
        /// This thread runs throughout the life of an instance of this class.
        /// Its responsibility is to move the motor, until its current location is equal to the setpoint.
        /// It does this, whenever the class is signalled there is a new setpoint. 
        /// </summary>
        private void RunMotor()
        {
            int lastPostitionPercentage = CurrentLocationPercentage;

            while (true)
            {
                // If the setpoint is off, activate the motor
                if (currentLocation != setPoint)
                {
                    activateMotor.Write(PinValue.High);
                }

                // Check for setpoint limits, and adjust:
                while (currentLocation != setPoint)
                {
                    // Count the motor position in one direction or the other
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

                    // Turn the moter to the next step
                    in1.Write(stepSequence0[currentStep]);
                    in2.Write(stepSequence1[currentStep]);
                    in3.Write(stepSequence2[currentStep]);
                    in4.Write(stepSequence3[currentStep]);

                    if (lastPostitionPercentage != CurrentLocationPercentage)
                    {
                        lastPostitionPercentage = CurrentLocationPercentage;
                        deviceData.CurrentLevel = CurrentLocationPercentage;
                        
                        if (transmitDataHandler != null)
                        {
                            transmitDataHandler();
                        }
                    }

                    Thread.Sleep(2);
                }
                
                // Deactivate the motor and wait for a new setpoint
                activateMotor.Write(PinValue.Low);
                newSetpointSignal.WaitOne();
            }
        }
    }
}
