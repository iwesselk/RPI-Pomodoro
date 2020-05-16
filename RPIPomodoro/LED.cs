using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace RPIPomodoro
{
    class LED
    {
        public enum State
        {
            Off,
            On,
            Blinking
        }
        public State myState { get; set; }
        public bool blinking => myState == State.Blinking || blinkCount > 0;
        public bool on => (blinkCount > 0 || myState == State.Blinking) ? blinkOn : myState == State.On;

        public int defaultBlinkCount = 3;

        private int blinkCount = 0;
        private bool blinkOn = false;
        private int pin;

        private GpioController controller;
        public LED(GpioController controller, int pin, State myState)
        {
            this.controller = controller;
            this.pin = pin;
            this.myState = myState;
            controller.OpenPin(pin, PinMode.Output);
        }
        public LED(GpioController controller, int pin) : this(controller, pin, State.Off) { }
        public bool IsBlinking()
        {
            return myState == State.Blinking || blinkCount > 0;
        }
        public void DoAction()
        {
            if (blinking)
            {
                if (blinkCount > 0)
                    blinkCount--;
                blinkOn = !blinkOn;
            }
            controller.Write(pin, on ? PinValue.High : PinValue.Low);
        }
        public void MakeBlink()
        {
            blinkCount = defaultBlinkCount;
            blinkOn = false;
        }
    }
}
