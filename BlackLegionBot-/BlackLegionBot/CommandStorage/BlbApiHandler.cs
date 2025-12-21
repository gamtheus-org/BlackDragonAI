using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;

namespace BlackLegionBot.CommandStorage
{
    public class BlbApiHandler
    {
        public event Action OnAuthenticated;

        private readonly IBlbApi _blbApi;
        private readonly BlbApiConfig _config;
        private string _jwt;
        private bool _hasBeenAuthenticated = false;

        public BlbApiHandler(IBlbApi blbApi, BlbApiConfig config)
        {
            this._blbApi = blbApi;
            this._config = config;

            config.AuthenticationChanged += OnAuthenticationTokenChanged;
        }

        public void OnAuthenticationTokenChanged(string jwt)
        {
            this._jwt = jwt;

            if(!_hasBeenAuthenticated)
                this.OnAuthenticated?.Invoke();
            this._hasBeenAuthenticated = true;
        }


        // Endpoints
        public async Task<AuthResult> Authenticate()
        {
            var authResult = await _blbApi.Authenticate(this._config);
            this._config.SetJwt(authResult.Token);
            return authResult;
        }

        public Task<IEnumerable<CommandDetails>> GetAllCommands() =>
            _blbApi.GetCommands(this._jwt);

        public Task<CommandDetails> CreateCommand(CommandDetails commandDetails) =>
            _blbApi.CreateCommand(this._jwt, commandDetails);

        public Task<CommandDetails> EditCommand(CommandDetails commandDetails) =>
            _blbApi.EditCommand(this._jwt, commandDetails);

        public Task DeleteCommand(string commandName) =>
            _blbApi.DeleteCommand(this._jwt, commandName);

        public Task AddAlias(string commandName, string alias) =>
            _blbApi.AddAlias(this._jwt, commandName, new AliasInput() { Alias = alias });

        public Task DeleteAlias(string alias) =>
            _blbApi.DeleteAlias(this._jwt, alias);

        public Task<TimedMessage> CreateTimedMessage(string command) =>
            _blbApi.CreateTimedMessage(this._jwt, new TimedMessage()
            {
                Command = command
            });

        public Task<IEnumerable<TimedMessage>> GetTimedMessages() =>
            _blbApi.GetTimedMessages(this._jwt);

        public Task DeleteTimedMessage(string command) =>
            _blbApi.DeleteTimedMessage(this._jwt, command);

        public Task SubscribeToWebhook(WebhookSubscription webhookSubscription) =>
            _blbApi.SubscribeToWebhook(this._jwt, webhookSubscription);

        public Task SubscribeToWebhookIdempotent(WebhookSubscription webhookSubscription) =>
            _blbApi.SubscribeToWebhookIdempotent(this._jwt, webhookSubscription);

        public Task<BlbCounter> IncrementDeathCount(string gameId) =>
            _blbApi.IncrementDeathCount(this._jwt, gameId);

        public Task<BlbCounter> DecrementDeathCount(string gameId) =>
            _blbApi.DecrementDeathCount(this._jwt, gameId);

        public Task<BlbCounter> UpdateDeathCount(BlbCounter blbCounter, string gameId) =>
            _blbApi.UpdateDeathCount(this._jwt, gameId, blbCounter);

        public Task<BlbCounter> GetDeathCount(string gameId) =>
            _blbApi.GetDeathCount(this._jwt, gameId);

        public Task<IEnumerable<BlbCounter>> GetAllDeathCounts() =>
            _blbApi.GetAllDeathCounts(this._jwt);

        public Task<IEnumerable<BlbCounter>> GetAllCounters() =>
            _blbApi.GetCounters(this._jwt);

        public Task DeleteCounter(string counterName) =>
            _blbApi.DeleteCounter(this._jwt, counterName);

        public Task<Existence> CounterExists(string counterName) =>
            _blbApi.CounterExists(this._jwt, counterName);
    }

    public static class BlbApiExtensions
    {
        public static ApiError ToBlbApiError(this ApiException apiException) =>
            JsonSerializer.Deserialize<ApiError>(apiException.Content);
    }
}
