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

        static int rollDownButtonPin = 32;
        static int rollUpButtonPin = 33;
        static int rollToButtomButtonPin = 22;
        static int rollToTopButtonPin = 23;

        static int StopAllActionButtonPin = 21;

        static int CalibrateButtonPin = 36;

        static int rollDownLEDIndicatorPinNumber = 18;
        static int rollUpLEDInticatiorPinNumber = 19;

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

        static Thread motorThread;


        public static void Main()
        {


            InitializeSystem();
            //CalibrateMotorController();

            //GpioPin in1 = gpioController.OpenPin(motorControllerIn1PinNumber, PinMode.Output);
            //GpioPin in2 = gpioController.OpenPin(motorControllerIn2PinNumber, PinMode.Output);
            //GpioPin in3 = gpioController.OpenPin(motorControllerIn3PinNumber, PinMode.Output);
            //GpioPin in4 = gpioController.OpenPin(motorControllerIn4PinNumber, PinMode.Output);

            //in1.Write(PinValue.Low);
            //in2.Write(PinValue.Low);
            //in3.Write(PinValue.Low);
            //in4.Write(PinValue.Low);

            int count = 0;
            int step = 0;

            // Program life 
            while (true)
            {
                while (rollDownButton.IsPressed)
                {
                    motorController.SetPoint++;
                    motorController.ResetEvent.Set();
                    Thread.Sleep(5);
                }
                
                while (rollUpButton.IsPressed)
                {
                    motorController.SetPoint--;
                    motorController.ResetEvent.Set();
                    Thread.Sleep(5);
                }




                //while (count < 5000)
                //{
                //    in1.Write(stepSequence0[step]);
                //    in2.Write(stepSequence1[step]);
                //    in3.Write(stepSequence2[step]);
                //    in4.Write(stepSequence3[step]);

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
                //    in1.Write(stepSequence0[step]);
                //    in2.Write(stepSequence1[step]);
                //    in3.Write(stepSequence2[step]);
                //    in4.Write(stepSequence3[step]);

                //    count--;
                //    step--;
                //    if (step < 0)
                //    {
                //        step = 3;
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
                                                  motorControllerIn4PinNumber);

            rollDownButton = new GpioButton(rollDownButtonPin, gpioController, false);
            rollUpButton = new GpioButton(rollUpButtonPin, gpioController, false);
            rollToButtomButton = new GpioButton(rollToButtomButtonPin, gpioController, false);
            rollToTopButton = new GpioButton(rollToTopButtonPin, gpioController, false);
            calibrateButton = new GpioButton(CalibrateButtonPin, gpioController, false);
            stopMotorButton = new GpioButton(StopAllActionButtonPin, gpioController, false);

            rollToTopButton.ButtonDown += RollToTopButton_ButtonDown;
            rollToButtomButton.ButtonDown += RollToButtomButton_ButtonDown;
            stopMotorButton.ButtonDown += StopMotorButton_ButtonDown;

            upLED = gpioController.OpenPin(rollUpLEDInticatiorPinNumber, PinMode.Output);
            downLED = gpioController.OpenPin(rollDownLEDIndicatorPinNumber, PinMode.Output);

            motorThread = new Thread(new ThreadStart(motorController.RunMotor));
            motorThread.Start();
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
                    motorController.ResetEvent.Set();
                    Thread.Sleep(5);
                }

                while (rollUpButton.IsPressed)
                {
                    motorController.SetPoint--;
                    motorController.ResetEvent.Set();
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
            motorController.SetPoint = motorController.CurrentState;
        }

        private static void RollToButtomButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MaxSetpoint;
            motorController.ResetEvent.Set();
        }

        private static void RollToTopButton_ButtonDown(object sender, EventArgs e)
        {
            motorController.SetPoint = motorController.MinSetpoint;
            motorController.ResetEvent.Set();
        }
    }
}







