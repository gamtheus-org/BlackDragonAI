using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BlackLegionBot.CommandHandling.SpecialsOperatorsHandling;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace BlackLegionBot.NonCommandBased
{
    public class ReconnectionManager
    {
        private readonly Action _reconnect;
        private int _reconnectionAttempts;
        private const int ReconnectWaitInMs = 5000;
        private Timer _timerToStartTimer;
        private Timer _reconnectTimer;
        private readonly Func<Task<bool>> _connectAsync;
        private readonly Func<Task> _disconnectAsync;
        private readonly TwitchClient _twitchClient;

        public ReconnectionManager(Action reconnect)
        {
            this._reconnect = reconnect;
        }

        public ReconnectionManager(TwitchClient client)
        {
            this._twitchClient = client;
            this._connectAsync = client.ConnectAsync;
            this._disconnectAsync = client.DisconnectAsync;

            var now = DateTime.Now;
            this._reconnectTimer = new Timer(Math.Abs((now.AddDays(1) - now).TotalMilliseconds))
            {
                AutoReset = true
            };
            this._reconnectTimer.Elapsed += (obj, args) =>
            {
                this.DisconnectAndReconnectAsync();
            };

            var timeTo0800 = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0).AddDays(1) - now;
            this._timerToStartTimer = new Timer(timeTo0800.TotalMilliseconds)
            {
                AutoReset = false
            };
            this._timerToStartTimer.Elapsed += (obj, args) =>
            {
                this._reconnectTimer.Start();
            };
            this._timerToStartTimer.Start();
        }

        public async Task DisconnectAndReconnectAsync()
        {
            await this._disconnectAsync();
            System.Threading.Thread.Sleep(1000);
            await this._connectAsync();
            System.Threading.Thread.Sleep(1000);
            await this._twitchClient.JoinChannelAsync("BlackDragon");
        }

        public void OnConnection(object sender, OnConnectedEventArgs args)
        {
            Console.WriteLine("Bot has successfully connected with Twitch chat");
            // this._reconnectionAttempts = 0;
        }

        public Task OnDisconnectAsync(object sender, OnDisconnectedArgs args)
        {
            Console.WriteLine("Bot has been disconnected from Twitch chat");
            // Console.WriteLine($"Wait until reconnect: {_reconnectionAttempts * ReconnectWaitInMs / 1000} seconds");
            // Thread.Sleep(_reconnectionAttempts * ReconnectWaitInMs);
            // this._reconnectionAttempts++;
            // Console.WriteLine($"Trying to reconnect. Attempt: {this._reconnectionAttempts}");
            // this._reconnect();
            return Task.CompletedTask;
        }
    }
}
