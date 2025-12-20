using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using Timer = System.Timers.Timer;

namespace BlackLegionBot.NonCommandBased
{
    public class TimedMessageHandler
    {
        private readonly Timer _timer;
        private readonly Timer _offsetTimer;
        private readonly LiveStatusManager _liveStatusManager;

        public TimedMessageHandler(int secondsBetweenEvents, string message, Func<string, Task> sendMessageAsync, LiveStatusManager liveStatusManager, int offsetInMinutes = 0) : 
            this(0, secondsBetweenEvents, message, sendMessageAsync,  liveStatusManager, offsetInMinutes)
        {
            
        }

        public TimedMessageHandler(int minutesBetweenEvents, int secondsBetweenEvents, string message, Func<string, Task> sendMessageAsync, LiveStatusManager liveStatusManager, int offsetInMinutes = 0)
        {
            this._liveStatusManager = liveStatusManager;
            // Sets up the timer
            this._timer = new Timer((minutesBetweenEvents * 60 + secondsBetweenEvents) * 1000)
            {
                AutoReset = true
            };
            this._timer.Elapsed += (sender, args) => SendMessageAsync(sendMessageAsync, message);


            if (offsetInMinutes > 0)
            {
                // Sets up a offset timer to start the base timer on a offset
                this._offsetTimer = new Timer(offsetInMinutes * 60 * 1000)
                {
                    AutoReset = false
                };
                this._offsetTimer.Elapsed += (sender, args) => StartTimer(sendMessageAsync, message);
                this._offsetTimer.Start();
            }
            else
            {
                StartTimer(sendMessageAsync, message);
            }
        }

        public void StopTimer() {
            this._timer.AutoReset = false;
            this._timer.Enabled = false;
            this._timer.Stop();
            this._timer.Dispose();
        }

        private async Task StartTimer(Func<string, Task>  sendMessageAsync, string message)
        {
            this._timer.Start();
            await SendMessageAsync(sendMessageAsync, message);
        }

        private async Task SendMessageAsync(Func<string, Task> sendMessageAsync, string message)
        {
            if (_liveStatusManager.IsLive().Result)
            {
                await sendMessageAsync(message);
            }
        }
    }
}
