//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using BlackLegionBot.CommandStorage;
//using TwitchLib.Api.Core.RateLimiter;
//using TwitchLib.Client.Events;
//
//namespace BlackLegionBot.CommandHandling
//{
//    public class CountersManager : ICommandHandler
//    {
//        private readonly BlbApiHandler _blbApiHandler;
//        private readonly Action<string> _sendMessage;
//        private readonly Regex _numericRegex = new Regex("[0-9]+");
////        private readonly int _maxCharacterAmount = 127;
//        private readonly Regex _counterNameRegex = new Regex("[a-zA-Z][a-zA-Z0-9]{0,126}");
//        private readonly Regex _counterCommandCallRegex = new Regex("![a-zA-Z][a-zA-Z0-9]{0,126}(\\+|\\-| ?[0-9]+){0,1}");
//
//        public CountersManager(BlbApiHandler blbApiHandler, Action<string> sendMessage)
//        {
//            this._blbApiHandler = blbApiHandler;
//            this._sendMessage = sendMessage;
//        }
//
//        public async void Handle(OnMessageReceivedArgs messageReceivedArgs)
//        {
//            var message = messageReceivedArgs.ChatMessage.Message;
//            message = message.TrimEnd();
//            if (!_counterCommandCallRegex.IsCompleteMatch(message.TrimEnd()))
//                return;
//
//            var counterName = _counterNameRegex.Match(message).Value;
//            if (! await CounterExists(counterName))
//            {
//                Console.WriteLine($"{counterName} does not exist");
//                return;
//            }
//
//            var messageIsJustTheCounterName = message.Length == counterName.Length + 1;
//            if (messageIsJustTheCounterName)
//                ShareCurrentCount(counterName);
//            if(message.Contains("+"))
//                IncrementCounter(counterName);
//            else if(message.Contains("-"))
//                DecrementCounter(counterName);
//            else
//                SetCounter(counterName, message);
//        }
//
//        public async Task<bool> CounterExists(string counterName) =>
//            (await _blbApiHandler.CounterExists(counterName)).Exists;
//
//        private async void ShareCurrentCount(string counterName)
//        {
//            var count = await _blbApiHandler.GetDeathCount(counterName);
//            _sendMessage($"De counter ${counterName} staat momenteel op {count}!");
//        }
//
//        private async void IncrementCounter(string counterName)
//        {
//            await _blbApiHandler.IncrementDeathCount(counterName);
//            ShareCurrentCount(counterName);
//        }
//
//        private async void DecrementCounter(string counterName)
//        {
//            await _blbApiHandler.DecrementDeathCount(counterName);
//            ShareCurrentCount(counterName);
//        }
//
//        private async void SetCounter(string counterName, string message)
//        {
//            var amount = int.Parse(CommonRegexes.Numerics.Match(message).Value);
//            await _blbApiHandler.UpdateDeathCount(new BlbCounter()
//            {
//                Deaths = amount,
//                GameId = counterName
//            }, counterName);
//            ShareCurrentCount(counterName);
//        }
//    }
//}
