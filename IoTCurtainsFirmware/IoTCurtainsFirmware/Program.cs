using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;
using System.Collections;

namespace IoTCurtainsFirmware
{
    public class Program
    {
        static int motorStepsPerRotation = 2038;

        static int rollDownButtonPin = 1;
        static int rollUpButtonPin = 22;
        static int rollToButtomButtonPin = 3;
        static int rollToTopButtonPin = 26;

        static int StopAllActionButtonPin = 21;

        static int CalibrateButtonPin = 36;

        static int rollDownLEDIndicatorPinNumber = 2;
        static int rollUpLEDInticatiorPinNumber = 15;

        static int motorControllerIn1PinNumber = 13;
        static int motorControllerIn2PinNumber = 12;
        static int motorControllerIn3PinNumber = 14;
        static int motorControllerIn4PinNumber = 27;

        static GpioController gpioController;
        static MotorController motorController;

        static GpioPin upLED;
        static GpioPin downLED;


        static GpioButton rollDownButton;
        static GpioButton rollUpButton;
        static GpioButton rollToButtomButton;
        static GpioButton rollToTopButton;
        static GpioButton calibrateButton;
        static GpioButton stopMotorButton;

        //static Thread motorThread;


        

        static int[,] stepSequence =  
        { 
            { 1, 0, 1, 0 }, 
            { 0, 1, 1, 0 }, 
            { 0, 1, 0, 1 }, 
            { 1, 0, 0, 1 } 
        };

        public static void Main()
        {

            InitializeSystem();
            //CalibrateMotorController();




            GpioPin in1 = gpioController.OpenPin(motorControllerIn1PinNumber, PinMode.Output);
            GpioPin in2 = gpioController.OpenPin(motorControllerIn2PinNumber, PinMode.Output);
            GpioPin in3 = gpioController.OpenPin(motorControllerIn3PinNumber, PinMode.Output);
            GpioPin in4 = gpioController.OpenPin(motorControllerIn4PinNumber, PinMode.Output);

            in1.Write(PinValue.High);
            in2.Write(PinValue.High);
            in3.Write(PinValue.High);
            in4.Write(PinValue.High);

            int count = 0;
            int step = 0;

            // Program life 
            while (true)
            {
                while (rollDownButton.IsPressed)
                {
                    downLED.Write(1);
                    //motorController.RollDown();
                    //motorThread.Resume();
                    //Thread.Sleep(5);
                }
                downLED.Write(0);

                while (rollUpButton.IsPressed)
                {
                    upLED.Write(1);
                    //motorController.RollUp();
                    //motorThread.Resume();
                    //Thread.Sleep(5);
                }
                upLED.Write(0);




                //while (count < 5000)
                //{
                //    in1.Write(stepSequence[step, 0]);
                //    in2.Write(stepSequence[step, 1]);
                //    in3.Write(stepSequence[step, 2]);
                //    in4.Write(stepSequence[step, 3]);

                //    count++;
                //    step++;
                //    if (step > 3)
                //    {
                //        step = 0;
                //    }
                //    Thread.Sleep(5);
                //}

                //while (count > 0)
                //{
                //    in1.Write(stepSequence[step, 0]);
                //    in2.Write(stepSequence[step, 1]);
                //    in3.Write(stepSequence[step, 2]);
                //    in4.Write(stepSequence[step, 3]);

                //    count--;
                //    step--;
                //    if (step < 0)
                //    {
                //        step = 0;
                //    }
                //    Thread.Sleep(5);
                //}
            }

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Initilizes the following System Components:
        /// 
        /// - GPIO Controller
        /// - MotorController
        /// - Input Buttons
        /// - LED's
        /// -
        /// 
        /// </summary>
        private static void InitializeSystem()
        {
            // Initialize
            gpioController = new GpioController(PinNumberingScheme.Board);

            motorController = new MotorController(gpioController,
                                                  motorControllerIn1PinNumber,
                                                  motorControllerIn2PinNumber,
                                                  motorControllerIn3PinNumber,
                                                  motorControllerIn4PinNumber,
                                                  motorStepsPerRotation);

            rollDownButton = new GpioButton(rollDownButtonPin, gpioController);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController);
            rollToButtomButton = new GpioButton(rollToButtomButtonPin, gpioController);
            rollToTopButton = new GpioButton(rollToTopButtonPin, gpioController);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController);

            rollToTopButton.ButtonDown += RollToTopButton_ButtonDown;
            rollToButtomButton.ButtonDown += RollToButtomButton_ButtonDown;
            stopMotorButton.ButtonDown += StopMotorButton_ButtonDown;

            upLED = gpioController.OpenPin(rollUpLEDInticatiorPinNumber, PinMode.Output);
            downLED = gpioController.OpenPin(rollDownLEDIndicatorPinNumber, PinMode.Output);

            //motorThread = new Thread(new ThreadStart(motorController.runMotor));
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
        private static void CalibrateMotorController()
        {
            bool topCalibrated = false;
            while (!motorController.Calibrated)
            {
                while (rollDownButton.IsPressed)
                {
                    motorController.SetPoint++;
                    motorController.RollDown();
                    Thread.Sleep(5);
                }

                while (rollUpButton.IsPressed)
                {
                    motorController.SetPoint--;
                    motorController.RollUp();
                    Thread.Sleep(5);
                }

                if (calibrateButton.IsPressed)
                {
                    while (calibrateButton.IsPressed) ;
                    if (!topCalibrated)
                    {
                        motorController.MinSetpoint = motorController.SetPoint;
                        topCalibrated = true;
                        continue;
                    }
                    else
                    {
                        motorController.MaxSetpoint = motorController.SetPoint;
                        motorController.Calibrated = true;
                    }
                }
            }
        }


        private static void StopMotorButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.Stop();
        }

        private static void RollToButtomButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MaxSetpoint;
        }

        private static void RollToTopButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MinSetpoint;
        }
    }
}
