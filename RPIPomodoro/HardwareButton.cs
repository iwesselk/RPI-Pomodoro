using System;
using System.Collections.Generic;
using System.Text;
using System.Device.Gpio;
namespace RPIPomodoro
{
    class HardwareButton
    {
        private GpioController controller;
        private int pin;
        private bool isDown = false;
        private bool lastCallState = false;
        public delegate bool ButtonTrigger(bool lastCallState);
        private ButtonTrigger myTrigger;
        public void ButtonRisingHandler(object sender, PinValueChangedEventArgs args) {
            isDown = true;
            this.lastCallState = myTrigger(this.lastCallState);
        }
        public void ButtonFallingHandler(object sender, PinValueChangedEventArgs args)
        {
            isDown = false;
        }
        public HardwareButton(GpioController controller, int pin, ButtonTrigger myTrigger)
        {
            this.controller = controller;
            this.pin = pin;
            controller.OpenPin(pin, PinMode.InputPullDown);
            controller.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Rising, ButtonRisingHandler);
            controller.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Falling, ButtonFallingHandler);
        }
    }
}
