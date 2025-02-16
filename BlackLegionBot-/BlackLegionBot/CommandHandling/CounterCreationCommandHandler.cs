using BlackLegionBot.CommandStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class CounterCreationCommandHandler : ICommandHandler
    {
        private readonly string _commandCall = "!newcounter ";
        private readonly BlbApiHandler _blbApi;
        private readonly Bot _bot;
        public event Action<string> OnCounterCreated;

        public CounterCreationCommandHandler(BlbApiHandler blbApi, Bot bot)
        {
            this._blbApi = blbApi;
            this._bot = bot;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            if (this._commandCall.Length >= messageReceivedArgs.ChatMessage.Message.Length) return;
            var commandName = messageReceivedArgs.ChatMessage.Message.
                Substring(this._commandCall.Length)
                .TrimEnd()
                .Replace("!", "");

            // Call blbApi
            var counter = new BlbCounter()
            {
                Deaths = 0,
                GameId = commandName,
                IsDeathCount = false
            };
            await this._blbApi.UpdateDeathCount(counter, commandName);
            this.OnCounterCreated?.Invoke(commandName);
            this._bot.SendMessageToChannel($"De counter {counter.GameId} is aangemaakt!");
        }
    }

    public class CounterDeletionCommandHandler : ICommandHandler
    {
        private readonly string _commandCall = "!deletecounter ";
        private readonly BlbApiHandler _blbApi;
        private readonly Bot _bot;
        public event Action<string> OnCounterDeleted;

        public CounterDeletionCommandHandler(BlbApiHandler blbApi, Bot bot)
        {
            this._blbApi = blbApi;
            this._bot = bot;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            if (this._commandCall.Length >= messageReceivedArgs.ChatMessage.Message.Length) return;
            var commandName = messageReceivedArgs.ChatMessage.Message.
                Substring(this._commandCall.Length)
                .TrimEnd()
                .Replace("!", "");

            // Call blbApi
            await this._blbApi.DeleteCounter(commandName);
            this.OnCounterDeleted?.Invoke(commandName);
            this._bot.SendMessageToChannel($"De counter {commandName} is verwijderd");
        }
    }

    public class CounterRetrievalCommandHandler : ICommandHandler
    {
        private readonly BlbApiHandler _blbApi;
        private readonly Bot _bot;
        private HashSet<string> _counters;

        public CounterRetrievalCommandHandler(BlbApiHandler handler, Bot bot)
        {
            this._blbApi = handler;
            this._bot = bot;
            this._blbApi.OnAuthenticated += SetupCounters;
        }

        private async void SetupCounters()
        {
            var counters = await this._blbApi.GetAllCounters();
            foreach (var counter in counters)
            {
                Console.WriteLine($"Exists: {counter}");
            }
            this._counters = counters.Select(c => c.GameId).ToHashSet();
        }

        public void AddCounter(string counterName)
        {
            this._counters.Add(counterName);
        }

        public void DeleteCounter(string counterName)
        {
            this._counters.Remove(counterName);
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var message = messageReceivedArgs.ChatMessage.Message;
            if (!message.Contains("!"))
                return;

            var counterName = new Regex("[a-zA-Z]+").Matches(message)[0].Value;
            if (!this._counters.Contains(counterName))
                return;

            int count;
            if (message.Contains("+"))
            {
                // Increment
                var counter = await this._blbApi.IncrementDeathCount(counterName);
                count = counter.Deaths;
            }
            else if (message.Contains("-"))
            {
                // Decrement
                var counter = await this._blbApi.DecrementDeathCount(counterName);
                count = counter.Deaths;
            }
            else
            {
                var amountMatch = new Regex(" [0-9]+").Match(message);
                if (amountMatch.Success)
                {
                    // Set to specified amount
                    var newAmount = int.Parse(amountMatch.Value.TrimStart());
                    await this._blbApi.UpdateDeathCount(new BlbCounter()
                    {
                        Deaths = newAmount,
                        GameId = counterName,
                        IsDeathCount = false
                    }, counterName);
                    count = newAmount;
                }
                else
                {
                    // Retrieve
                    var counter = await this._blbApi.GetDeathCount(counterName);
                    count = counter.Deaths;
                }
            }

            this._bot.SendMessageToChannel($"De {counterName} teller staat op {count}!");
        }
    }
}
