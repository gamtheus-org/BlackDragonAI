using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;

namespace BlackLegionBot.NonCommandBased
{
    public class WebhookHandler
    {
        private const string WebhookPath = "/bot/webhook/";
        
        public event Action CommandsChanged;
        public event Action TimedMessagesChanged;

        public event Action<string> AuthTokenChanged;

        private readonly BlbApiHandler _apiClient;
        private readonly List<(string webhookPath, Action eventToRaise)> _webhooks = new List<(string webhookPath, Action eventToRaise)>();

        public WebhookHandler(BlbApiHandler apiClient, Action reconnect)
        {
            this._apiClient = apiClient;
            _webhooks.Add(("commands", RaiseCommandChangedEvent));
            _webhooks.Add(("timedmessages", RaiseTimedMessagesChangedEvent));
            _webhooks.Add(("reconnect", reconnect));
        }

        public async void ListenForWebhooks()
        {
            var defaultWebhookUrl = new WebhookSubscription()
            {
                Url = $"https://blackdragonai.nl{WebhookPath[..^1]}"
            };
            await this._apiClient.SubscribeToWebhookIdempotent(defaultWebhookUrl);
            var listener = SetListener();

            while (true)
            {
                await HandleWebhookCall(listener);
            }
        }

        private async Task HandleWebhookCall(HttpListener listener)
        {
            var request = await listener.GetContextAsync();
            Console.WriteLine($"Received webhook call: {request.Request.RawUrl}");
            var path = request.Request.RawUrl;
            if (path.Contains("/authorized"))
            {
                await using var stream = request.Request.InputStream;
                using var reader = new StreamReader(stream);
                var authToken = await reader.ReadToEndAsync();

                AuthTokenChanged?.Invoke(authToken);
                return;
            }
            foreach (var webhook in _webhooks.Where(webhook => path.Contains(webhook.webhookPath)))
            {
                webhook.eventToRaise();
            }
        }

        private HttpListener SetListener()
        {const string WebhookPath = "/bot/webhook/";
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://*:80{WebhookPath}");
            listener.Start();
            Console.WriteLine("Listening");
            return listener;
        }

        private void RaiseCommandChangedEvent() => CommandsChanged?.Invoke();
        private void RaiseTimedMessagesChangedEvent() => TimedMessagesChanged?.Invoke();
    }
}
