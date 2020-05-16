using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Device.Gpio;
using System.Runtime.CompilerServices;

namespace RPIPomodoro
{
    class LedController
    {
        public const int SleepTime = 250;
        public const int BlinkTimes = 3;
        private static int BlinksRemaining = 0;
        private static LED BlinkingLED = LED.None;
        private static bool BlinkOn = false;
        private static LED led = LED.FreeLED;
        static public GpioController controller { get; set; }
        public enum LED
        {
            WorkLED,
            BreakLED,
            FreeLED,
            None
        }

        public static void ThreadProc()
        {
            while (true)
            {
                if (BlinksRemaining > 0)
                {
                    if (!BlinkOn)
                    {
                        Console.WriteLine("Blinking on");
                        BlinkOn = true;
                        controller.Write(Program.pin1, BlinkingLED == LED.WorkLED || BlinkingLED == LED.None ? PinValue.High : PinValue.Low);
                        controller.Write(Program.pin2, BlinkingLED == LED.BreakLED || BlinkingLED == LED.None ? PinValue.High : PinValue.Low);
                        controller.Write(Program.pin3, BlinkingLED == LED.FreeLED || BlinkingLED == LED.None ? PinValue.High : PinValue.Low);
                        BlinksRemaining -= 1;
                    } else
                    {
                        Console.WriteLine("Blinking off");
                        BlinkOn = false;
                        controller.Write(Program.pin1, PinValue.Low);
                        controller.Write(Program.pin2, PinValue.Low);
                        controller.Write(Program.pin3, PinValue.Low);
                    }
                } else
                {
                    Console.WriteLine("LED Set on");
                    controller.Write(Program.pin1, led == LED.WorkLED ? PinValue.High : PinValue.Low);
                    controller.Write(Program.pin2, led == LED.BreakLED ? PinValue.High : PinValue.Low);
                    controller.Write(Program.pin3, led == LED.FreeLED ? PinValue.High : PinValue.Low);
                }
                Thread.Sleep(SleepTime);
            }
        }
        private static void InitBlink()
        {
            if (BlinksRemaining == 0)
            {
                BlinkOn = false;
                BlinksRemaining = BlinkTimes;
            }
        }
        public static void BlinkAll()
        {
            InitBlink();
            BlinkingLED = LED.None;
        }
        public static void BlinkWork()
        {
            InitBlink();
            BlinkingLED = LED.WorkLED;
        }
        public static void BlinkBreak()
        {
            InitBlink();
            BlinkingLED = LED.BreakLED;
        }
        public static void BlinkFree()
        {
            InitBlink();
            BlinkingLED = LED.FreeLED;
        }
        public static void Blink(LED l)
        {
            switch (l)
            {
                case LED.BreakLED:
                    BlinkBreak();
                    break;
                case LED.WorkLED:
                    BlinkWork();
                    break;
                case LED.FreeLED:
                    BlinkFree();
                    break;
            }
        }
        public static void SetLED(LED led)
        {
            LedController.led = led;
        }
        //DO NOT CALL FROM THE LED CONTROLLER THREAD
        public static void WaitBlink()
        {
            while (BlinksRemaining > 0)
            {
                Console.WriteLine("Waiting Blinks");
                Thread.Sleep(SleepTime);
            }
            Console.WriteLine("Done Waiting Blinks");
        }
    }
}
