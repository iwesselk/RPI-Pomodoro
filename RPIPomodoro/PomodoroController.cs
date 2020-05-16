using System;
using System.Data;
using System.Device.Gpio;
using System.Threading;

namespace RPIPomodoro
{
    class PomodoroController
    {
        const int SleepTime = 250;
        private static DateTime? FutureDate;
        private static DateTime? Timeout;
        private static bool buttonHeld = false;
        private static State myState = State.Free;
        static public GpioController controller { get; set; }

        enum State
        {
            Working,
            Break,
            Free,
            NoChange
        }
        private static State HasNewState()
        {
            if (controller.Read(Program.button1) == 1 && !buttonHeld)
            {
                buttonHeld = true;
                return State.Working;
            }
            if (controller.Read(Program.button2) == 1 && !buttonHeld)
            {
                buttonHeld = true;
                return State.Break;
            }
            if (controller.Read(Program.button3) == 1 && !buttonHeld)
            {
                buttonHeld = true;
                return State.Free;
            }
            return State.NoChange;
        }
        private static bool AreButtonsHeld()
        {
            return controller.Read(Program.button1) == 1 || controller.Read(Program.button2) == 1 || controller.Read(Program.button3) == 1;
        }
        private static DateTime? SetTime(State s)
        {
            switch (s)
            {
                case State.Working:
                    return DateTime.Now.Add(new TimeSpan(0, minutes:30, 0));
                case State.Break:
                    return DateTime.Now.Add(new TimeSpan(0, minutes:5, 0));
                case State.Free:
                    return null;
            }
            return null;
        }
        private static void UpdateState(State newState)
        {
            myState = newState;
            FutureDate = SetTime(myState);
            LedController.Blink((LedController.LED)newState);
        }
        public static void ThreadProc()
        {
            while (true)
            {
                if (buttonHeld && !AreButtonsHeld())
                    buttonHeld = false;
                else if (buttonHeld)
                {
                    LedController.BlinkAll();
                    LedController.WaitBlink();
                    continue;
                }
                State newState = HasNewState();
                // We want to consider switching states
                if ((newState != myState) && (newState != State.NoChange))
                {
                    // We aren't currently timing, continue changing state
                    if (!Timeout.HasValue && !FutureDate.HasValue)
                    {
                        Console.WriteLine("We have a new state and no times");
                        UpdateState(newState);
                    } else if (Timeout.HasValue) 
                    {
                        Console.WriteLine("We have a new state but are in timeout");
                        // We have a timeout already and have passed it
                        if (DateTime.Now >= Timeout)
                        {
                            Console.WriteLine("We no longer have timeout");
                            Timeout = null;
                            UpdateState(newState);
                        }
                    } else
                    {
                        Console.WriteLine("Set a timeout");
                        // We have no timeout, but we have FutureDate, lets set a timeout.
                        Timeout = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, milliseconds:LedController.BlinkTimes*LedController.SleepTime));
                        LedController.BlinkAll();
                    }
                }
                if (FutureDate <= DateTime.Now)
                {
                    FutureDate = null;
                }
                if ((myState == State.Break || myState == State.Working) && !FutureDate.HasValue)
                {
                    LedController.BlinkAll();
                }
                if (FutureDate.HasValue)
                {
                    var x = FutureDate - DateTime.Now;
                    Console.WriteLine(x.ToString());
                }
                LedController.SetLED((LedController.LED)myState);
                Console.WriteLine($"My state is {myState}");
                Thread.Sleep(SleepTime);
            }
        }
    }
}
