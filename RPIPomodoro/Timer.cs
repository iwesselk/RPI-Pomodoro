using System;
using System.Collections.Generic;
using System.Text;

namespace RPIPomodoro
{
    class Timer
    {
        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan length;
        public bool expired => DateTime.Now > stopTime;
        public bool running => !expired;
        public Timer(TimeSpan length)
        {
            this.length = length;
            // We don't want it to auto start
            this.startTime = DateTime.Now;
            this.stopTime = DateTime.Now;
        }
        public void Reset()
        {
            this.startTime = DateTime.Now;
            this.stopTime = this.startTime.Add(length);
        }
    }
}
