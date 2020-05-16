using System;
using System.Device.Gpio;
using System.Threading;

namespace RPIPomodoro
{
    class Program
    {
        public const int pin1 = 2;
        public const int pin2 = 3;
        public const int pin3 = 4;
        public const int button1 = 5;
        public const int button2 = 6;
        public const int button3 = 7;
        const int timeout = 500;
        static void CheckForButtons(GpioController controller)
        {
            if (controller.Read(button1) == 1)
                Console.WriteLine("Button1 is triggered");
            if (controller.Read(button2) == 1)
                Console.WriteLine("Button2 is triggered");
            if (controller.Read(button3) == 1)
                Console.WriteLine("Button3 is triggered");
        }
        static void InitInputs(GpioController controller)
        {
            controller.OpenPin(pin1, PinMode.Output);
            controller.OpenPin(pin2, PinMode.Output);
            controller.OpenPin(pin3, PinMode.Output);

            controller.OpenPin(button1, PinMode.InputPullDown);
            controller.OpenPin(button2, PinMode.InputPullDown);
            controller.OpenPin(button3, PinMode.InputPullDown);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting pomodoro app");
            using (GpioController controller = new GpioController())
            { 
                InitInputs(controller);
                LedController.controller = controller;
                Thread LedProcess = new Thread(new ThreadStart(LedController.ThreadProc));
                PomodoroController.controller = controller;
                Thread PomodoroProcess = new Thread(new ThreadStart(PomodoroController.ThreadProc));
                LedProcess.Start();
                PomodoroProcess.Start();
                LedProcess.Join();
                PomodoroProcess.Join();
            }
            Console.WriteLine("Stopping pomodoro app");
            
        }
        static void oldMain()
        {
            //Console.WriteLine("Hello World!");
            Console.WriteLine("Lets blink the LED's");
            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(pin1, PinMode.Output);
                controller.OpenPin(pin2, PinMode.Output);
                controller.OpenPin(pin3, PinMode.Output);

                controller.OpenPin(button1, PinMode.InputPullDown);
                controller.OpenPin(button2, PinMode.InputPullDown);
                controller.OpenPin(button3, PinMode.InputPullDown);

                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    controller.Dispose();
                };

                while (true)
                {
                    Console.WriteLine($"Pin1 - {pin1}");
                    CheckForButtons(controller);
                    controller.Write(pin1, PinValue.High);
                    controller.Write(pin2, PinValue.Low);
                    controller.Write(pin3, PinValue.Low);
                    Thread.Sleep(timeout);
                    Console.WriteLine($"Pin1 - {pin2}");
                    CheckForButtons(controller);
                    controller.Write(pin1, PinValue.Low);
                    controller.Write(pin2, PinValue.High);
                    controller.Write(pin3, PinValue.Low);
                    Thread.Sleep(timeout);
                    Console.WriteLine($"Pin1 - {pin3}");
                    CheckForButtons(controller);
                    controller.Write(pin1, PinValue.Low);
                    controller.Write(pin2, PinValue.Low);
                    controller.Write(pin3, PinValue.High);
                    Thread.Sleep(timeout);
                }

            }
        }
    }
}
