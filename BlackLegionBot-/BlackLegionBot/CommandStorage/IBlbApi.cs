using BlackLegionBot.Credentials;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackLegionBot.CommandStorage
{
    public interface IBlbApi
    {
        [Post("/users/login")]
        Task<AuthResult> Authenticate([Body] BlbApiConfig config);

        [Get("/commands")]
        Task<IEnumerable<CommandDetails>> GetCommands([Header("X-Access-Token")] string authToken);

        [Post("/commands")]
        Task<CommandDetails> CreateCommand([Header("X-Access-Token")] string authToken, [Body] CommandDetails commandDetails);

        [Put("/commands")]
        Task<CommandDetails> EditCommand([Header("X-Access-Token")] string authToken, [Body] CommandDetails commandDetails);

        [Delete("/commands/{commandName}")]
        Task DeleteCommand([Header("X-Access-Token")] string authToken, string commandName);

        [Post("/commands/alias/{commandName}")]
        Task AddAlias([Header("X-Access-Token")] string authToken, string commandName, [Body] AliasInput aliasInput);

        [Delete("/commands/alias/{alias}")]
        Task DeleteAlias([Header("X-Access-Token")] string authToken, string alias);

        [Post("/timedmessages")]
        Task<TimedMessage> CreateTimedMessage([Header("X-Access-Token")] string authToken, [Body] TimedMessage timedMessage);

        [Get("/timedmessages")]
        Task<IEnumerable<TimedMessage>> GetTimedMessages([Header("X-Access-Token")] string authToken);

        [Delete("/timedmessages/{commandName}")]
        Task DeleteTimedMessage([Header("X-Access-Token")] string authToken, string commandName);

        [Post("/webhook")]
        Task<WebhookSubscriber> SubscribeToWebhook([Header("X-Access-Token")] string authToken, [Body] WebhookSubscription webhookSubscription);

        [Post("/webhook/idempotent")]
        Task<WebhookSubscriber> SubscribeToWebhookIdempotent([Header("X-Access-Token")] string authToken, [Body] WebhookSubscription webhookSubscription);

        [Post("/deaths/{gameId}")]
        Task<BlbCounter> IncrementDeathCount([Header("X-Access-Token")] string authToken, string gameId);

        [Put("/deaths/{gameId}")]
        Task<BlbCounter> UpdateDeathCount([Header("X-Access-Token")] string authToken, string gameId, [Body] BlbCounter blbCounter);

        [Get("/deaths/counters")]
        Task<IEnumerable<BlbCounter>> GetCounters([Header("X-Access-Token")] string authToken);

        [Get("/deaths/counters/{counterName}")]
        Task<BlbCounter> GetCounter([Header("X-Access-Token")] string authToken, string counterName);

        [Delete("/deaths/{gameId}")]
        Task<BlbCounter> DecrementDeathCount([Header("X-Access-Token")] string authToken, string gameId);

        [Get("/deaths/{gameId}")]
        Task<BlbCounter?> GetDeathCount([Header("X-Access-Token")] string authToken, string gameId);

        [Get("/deaths")]
        Task<IEnumerable<BlbCounter>> GetAllDeathCounts([Header("X-Access-Token")] string authToken);

        [Delete("/deaths/counters/{counterName}")]
        Task DeleteCounter([Header("X-Access-Token")] string authToken, string counterName);

        [Get("/deaths/exists/{counterName}")]
        Task<Existence> CounterExists([Header("X-Access-Token")] string authToken, string counterName);
    }

    public class AliasInput
    {
        public string Alias { get; set; }
    }

    public class BlbApiConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string JWT { get; private set; }

        public event Action<string> AuthenticationChanged;

        public BlbApiConfig() { }

        public BlbApiConfig(IConfigurationSection config)
        {
            this.Username = config.GetValue<string>("username");
            this.Password = config.GetValue<string>("password");
            this.Url = config.GetValue<string>("url");
            Console.WriteLine($"Username: {Username}, Password: {Password}, Url: {Url}");
        }

        public void SetJwt(string jwt)
        {
            this.JWT = jwt;
            AuthenticationChanged?.Invoke(this.JWT);
        }

        public async Task UpdateAppsettings()
        {
            var appsettings = await Appsettings.GetAppsettings();
            appsettings.BlbApi = this;
            await appsettings.WriteAppsettings();
        }
    }

    public class AuthResult
    {
        public string Token { get; set; }
        public int AuthorizationLevel { get; set; }
    }

    public class ApiError
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string DateTime { get; set; }
    }
}