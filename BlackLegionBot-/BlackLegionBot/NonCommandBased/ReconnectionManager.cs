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

namespace BlackLegionBot.NonCommandBased
{
    public class ReconnectionManager
    {
        private readonly Action _reconnect;
        private int _reconnectionAttempts;
        private const int ReconnectWaitInMs = 5000;
        private Timer _timerToStartTimer;
        private Timer _reconnectTimer;
        private readonly Func<bool> _connect;
        private readonly Action _disconnect;
        private readonly TwitchClient _twitchClient;

        public ReconnectionManager(Action reconnect)
        {
            this._reconnect = reconnect;
        }

        public ReconnectionManager(TwitchClient client)
        {
            this._twitchClient = client;
            this._connect = client.Connect;
            this._disconnect = client.Disconnect;

            var now = DateTime.Now;
            this._reconnectTimer = new Timer(Math.Abs((now.AddDays(1) - now).TotalMilliseconds))
            {
                AutoReset = true
            };
            this._reconnectTimer.Elapsed += (obj, args) =>
            {
                this.DisconnectAndReconnect();
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

        public void DisconnectAndReconnect()
        {
            this._disconnect();
            System.Threading.Thread.Sleep(1000);
            this._connect();
            System.Threading.Thread.Sleep(1000);
            this._twitchClient.JoinChannel("BlackDragon");
        }

        public void OnConnection(object sender, OnConnectedArgs args)
        {
            Console.WriteLine("Bot has successfully connected with Twitch chat");
            // this._reconnectionAttempts = 0;
        }

        public void OnDisconnect(object sender, OnDisconnectedEventArgs args)
        {
            Console.WriteLine("Bot has been disconnected from Twitch chat");
            // Console.WriteLine($"Wait until reconnect: {_reconnectionAttempts * ReconnectWaitInMs / 1000} seconds");
            // Thread.Sleep(_reconnectionAttempts * ReconnectWaitInMs);
            // this._reconnectionAttempts++;
            // Console.WriteLine($"Trying to reconnect. Attempt: {this._reconnectionAttempts}");
            // this._reconnect();
        }
    }
}
