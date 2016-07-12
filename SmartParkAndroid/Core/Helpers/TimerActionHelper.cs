using System;
using System.Timers;

namespace SmartParkAndroid.Core.Helpers
{
    public class TimerActionHelper
    {
        private readonly Timer _timer;
        private int _elapsedCount;

        public TimerActionHelper()
        {
            _elapsedCount = 0;
            _timer = new Timer {Interval = 1000};
        }

        public void Do(Action funcAfter, Func<int, bool> funcElapsedSingle, int seconds)
        {
            _timer.Elapsed += (sender, e) =>
            {
                ++_elapsedCount;
                timerHandler(sender, e, funcAfter, funcElapsedSingle, seconds);
            };

            _timer.Start(); 
        }

        private void timerHandler(object sender, EventArgs e, Action funcAfter, Func<int, bool> funcElapsedSingle, int seconds)
        {
            if (seconds != _elapsedCount)
            {
                funcElapsedSingle(_elapsedCount);
            }
            else
            {
                funcAfter();
                _timer.Stop();
            }
        }
    }
}