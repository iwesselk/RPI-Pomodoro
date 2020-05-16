using System;
using System.Device.Gpio;
using System.Linq.Expressions;
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
        public static Timer workingTimer = null;
        public static Timer breakTimer = null;
        public static Timer freeTimer = null;

        public static LED workingLed = null;
        public static LED breakLed = null;
        public static LED freeLed = null;
        public static HardwareButton workingButton = null;
        public static HardwareButton breakButton = null;
        public static HardwareButton freeButton = null;
        public static bool AreTimersRunning()
        {
            // Note: We don't care if freeTimer is running
            return workingTimer?.running == true || breakTimer?.running == true;
        }
        public static void MakeAllLEDSBlinkShort()
        {
            workingLed.MakeBlink();
            breakLed.MakeBlink();
            freeLed.MakeBlink();
        }
        public static void SetAllLEDSState(LED.State s)
        {
            workingLed.myState = s;
            breakLed.myState = s;
            freeLed.myState = s;
        }
        public static HardwareButton.ButtonTrigger makeDelegate(Timer t, LED l)
        {
            return delegate (bool lastCallState)
            {
                if (AreTimersRunning() && lastCallState)
                {
                    MakeAllLEDSBlinkShort();
                    return true;
                }
                t.Reset();
                SetAllLEDSState(LED.State.Off);
                l.myState = LED.State.On;
                return false;
            };
        }
        static void Main(string[] args)
        {
            using (GpioController controller = new GpioController())
            {
                workingLed = new LED(controller, pin1);
                breakLed = new LED(controller, pin2);
                freeLed = new LED(controller, pin3);
                workingTimer = new Timer(new TimeSpan(0, 30, 0));
                breakTimer = new Timer(new TimeSpan(0, 5, 0));
                freeTimer = new Timer(new TimeSpan(0, 0, 0));
                workingButton = new HardwareButton(controller, button1, makeDelegate(workingTimer, workingLed));
                breakButton = new HardwareButton(controller, button1, makeDelegate(breakTimer, breakLed));
                freeButton = new HardwareButton(controller, button1, makeDelegate(freeTimer, freeLed));
                while (true)
                {
                    if (workingTimer?.expired == true || breakTimer?.expired == true)
                        SetAllLEDSState(LED.State.Blinking);
                    workingLed.DoAction();
                    breakLed.DoAction();
                    freeLed.DoAction();
                    Thread.Sleep(250);
                }
            }
        }
    }
}
