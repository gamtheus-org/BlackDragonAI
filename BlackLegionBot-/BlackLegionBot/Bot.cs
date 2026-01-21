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
using BlackLegionBot.Helpers;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using Microsoft.Extensions.Hosting;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Models;

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
            var clientOptions = new ClientOptions(new ReconnectionPolicy(reconnectInterval: 10, maxAttempts: 15),
                clientType: ClientType.Chat);
            var webSocketClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(webSocketClient);

            _liveStatusManager = new LiveStatusManager(_twitchApi);
            _commercialManager = new CommercialManager(twitchApi, _liveStatusManager);
            CommandSelector = new CommandSelector(this, _twitchApi, commandRetriever, blbApi, cooldownManager, _commercialManager, TimeoutUserInChannelAsync);

            // EventHandlers
            Client.OnMessageReceived += async (obj, args) =>
            {
                Console.WriteLine($"{args.ChatMessage.DisplayName}: {args.ChatMessage.Message}");
                await CommandSelector.HandleCommand(obj, args);
            };
            Client.OnJoinedChannel += (sender, args) => SendMessageToChannelAsync("Joined channel");

            Client.Initialize(creds, this._userInfo.ChannelName);
            this._twitchApi.AuthManager.WhisperNeedsToBeSendAsync += SendAsyncWhisperToChannelAsync;
            _timedMessageManager = new TimedMessageManager(commandRetriever, blbApi, SendMessageToChannelAsync);

            // var viewerEventsHandlers = new ViewerEventsHandlers(SendMessageToChannelAsync);
            // var pubSubClient = new TwitchPubSub();
            // pubSubClient.Connect();
            // pubSubClient.OnFollow += viewerEventsHandlers.HandleFollowEvent;
            // pubSubClient.OnChannelSubscription += viewerEventsHandlers.HandleSubEvent;

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

                return Task.CompletedTask;
            };
            Client.OnDisconnected += _reconnectionManager.OnDisconnectAsync;
            Client.OnError += (sender, args) => { 
                Console.WriteLine("Error occurred: \n" + args.Exception);
                return Task.CompletedTask;
            };
            Client.OnFailureToReceiveJoinConfirmation += (sender, args) =>
            {
                Console.WriteLine("Failed to join chat");
                return Task.CompletedTask;
            };
            Client.OnConnectionError += (sender, args) =>
            {
                Console.WriteLine($"Connection error at {DateTime.Now}");
                return Task.CompletedTask;
            };
            Client.OnLeftChannel += (sender, args) =>
            {
                Console.WriteLine($"Client left channel at {DateTime.UtcNow}");
                return Task.CompletedTask;
            };

            _webhookHandler = new WebhookHandler(blbApi, () => this._reconnectionManager.DisconnectAndReconnectAsync());
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

        private async Task Connect()
        {
            // blb api
            await _blbApi.Authenticate();

            // twitch irc
            await Client.ConnectAsync();
        }

        public async Task SendMessageToChannelAsync(string message)
        {
            if (!message.TrimStart()[0].Equals('/'))
            {
                message = $"/me {message}";
            }

            await this.Client.SendMessageAsync(this._userInfo.ChannelName, message);
        }

        public Task SendAsyncWhisperToChannelAsync(string message) =>
            SendWhisperToChannelAsync(message, "gamtheus");
        
        public Task SendWhisperToChannelAsync(string message, string whisperTo) =>
            SendMessageToChannelAsync($"/w {whisperTo} {message}");

        private async Task ReconnectAsync(int attempts = 0)
        {
            await Task.Delay(attempts * 5 * 1000);
            await this.Client.ReconnectAsync();
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

        public async Task TimeoutUserInChannelAsync(string username, TimeSpan duration)
        {
            await this.Client.SendMessageAsync(this._userInfo.ChannelName,
                $"/timeout {username} {duration.TotalSeconds}");
            // await Client.TimeoutUserAsync(_userInfo.ChannelName, username, duration);
        }
    }
}