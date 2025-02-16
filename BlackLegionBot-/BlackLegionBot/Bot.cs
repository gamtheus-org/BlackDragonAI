using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlackLegionBot.CommandHandling;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.Credentials;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using Microsoft.Extensions.Hosting;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace BlackLegionBot
{
    public class Bot : IHostedService
    {
        private readonly UserInfo _userInfo;

        public static readonly IEnumerable<string> NamesOfAdmins = new string[]
        {
            "blackdragon", "gamtheus"
        };

        private readonly TwitchApiManager _twitchApi;
        private TwitchClient Client { get; }
        private CommandSelector CommandSelector { get; }
        private readonly BlbApiHandler _blbApi;
        private readonly TimedMessageManager _timedMessageManager;
        private readonly WebhookHandler _webhookHandler;
        private readonly CommercialManager _commercialManager;
        private readonly LiveStatusManager _liveStatusManager;
        private readonly ReconnectionManager _reconnectionManager;
        private readonly TwitchAuthManager _twitchAuthManager;

        public Bot(BlbApiHandler blbApi, ICommandRetriever commandRetriever, TwitchApiManager twitchApi, UserInfo userInfo, 
            IrcCredentials ircCredentials, CooldownManager cooldownManager, TwitchAuthManager twitchAuthManager)
        {
            this._userInfo = userInfo;
            _twitchAuthManager = twitchAuthManager;
            this._twitchApi = twitchApi;
            _blbApi = blbApi;

            var creds = new ConnectionCredentials(ircCredentials.Username, ircCredentials.AccessToken);
            var webSocketClient = new WebSocketClient(new ClientOptions()
            {
                ClientType = ClientType.Chat,
                ReconnectionPolicy = new ReconnectionPolicy(reconnectInterval: 10, maxAttempts: 15)
            });
            Client = new TwitchClient(webSocketClient) {AutoReListenOnException = true};
            Client.OnLog += (obj, args) =>
            {
                Console.WriteLine($"TwitchClient: {args.DateTime} | {args.Data}");
            };

            this._liveStatusManager = new LiveStatusManager(_twitchApi);
            this._commercialManager = new CommercialManager(twitchApi, _liveStatusManager);
            CommandSelector = new CommandSelector(this, _twitchApi, commandRetriever, blbApi, cooldownManager, _commercialManager);

            // EventHandlers
            Client.OnMessageReceived += async (obj, args) =>
            {
                Console.WriteLine($"{args.ChatMessage.DisplayName}: {args.ChatMessage.Message}");
                await CommandSelector.HandleCommand(obj, args);
            };
            Client.OnJoinedChannel += (sender, args) => SendMessageToChannel("Joined channel");

            Client.Initialize(creds, this._userInfo.ChannelName);
            this._twitchApi.AuthManager.WhisperNeedsToBeSend += SendWhisperToChannel;
            _timedMessageManager = new TimedMessageManager(commandRetriever, blbApi, SendMessageToChannel);

            var viewerEventsHandlers = new ViewerEventsHandlers(SendMessageToChannel);
            var pubSubClient = new TwitchPubSub();
            pubSubClient.Connect();
            pubSubClient.OnFollow += viewerEventsHandlers.HandleFollowEvent;
            pubSubClient.OnChannelSubscription += viewerEventsHandlers.HandleSubEvent;

            // Connection issues
            this._reconnectionManager = new ReconnectionManager(Client);
            Client.OnConnected += (sender, args) => 
            {
                this._reconnectionManager.OnConnection(sender, args);
                Console.WriteLine($"Joined channels: {this.Client.JoinedChannels.Count}");
                foreach(var channel in this.Client.JoinedChannels)
                {
                    Console.WriteLine("Joined channels: " + channel.Channel);
                }
            };
            Client.OnDisconnected += this._reconnectionManager.OnDisconnect;
            Client.OnError += (sender, args) => { Console.WriteLine("Error occurred: \n" + args.Exception); };
            Client.OnFailureToReceiveJoinConfirmation += (sender, args) => { Console.WriteLine("Failed to join chat"); };
            Client.OnConnectionError += (sender, args) => { Console.WriteLine($"Connection error at {DateTime.Now}"); };
            Client.OnLeftChannel += (sender, args) =>
            {
                Console.WriteLine($"Client left channel at {DateTime.UtcNow}");
            };

            _webhookHandler = new WebhookHandler(blbApi, this._reconnectionManager.DisconnectAndReconnect);
            _webhookHandler.CommandsChanged += () =>
            {
                Console.WriteLine("Retrieving commands because webhook");
                commandRetriever.RetrieveCommands();
            };
            _webhookHandler.TimedMessagesChanged += async () =>
            {
                Console.WriteLine("Retrieving timed messages because webhook");
                await _timedMessageManager.Start(this._liveStatusManager);
            };
            _webhookHandler.AuthTokenChanged += async authToken =>
            {
                Console.WriteLine($"Processing auth token: {authToken}");
                await _twitchAuthManager.UseAuthorizationToken(authToken);
            };
        }

        public async Task Connect()
        {
            // blb api
            await _blbApi.Authenticate();

            // twitch irc
            Client.Connect();
        }

        public void SendMessageToChannel(string message)
        {
            if (!message.TrimStart()[0].Equals('/'))
                message = $"/me {message}";
            this.Client.SendMessage(this._userInfo.ChannelName, message);
        }

        public void SendWhisperToChannel(string message) =>
            SendWhisperToChannel(message, "gamtheus");
        
        public void SendWhisperToChannel(string message, string whisperTo) =>
            SendMessageToChannel($"/w {whisperTo} {message}");

        public void TimeoutUser(string user, int duration, string message = "")
        {
            Client.TimeoutUser(this._userInfo.ChannelName, user, new TimeSpan(0, duration / 60, duration % 60), message);
        }

        private void Reconnect(int attempts = 0)
        {
            Thread.Sleep(attempts * 5 * 1000);
            this.Client.Reconnect();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Connect();

            await this._twitchApi.Initialize();

            this._webhookHandler.ListenForWebhooks();
            await _timedMessageManager.Start(this._liveStatusManager);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}