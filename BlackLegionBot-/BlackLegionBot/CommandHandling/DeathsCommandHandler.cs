using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;
using Refit;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class DeathsCommandHandler : ICommandHandler
    {
        private readonly BlbApiHandler _blbApiHandler;
        private readonly TwitchApiManager _twitchApiManager;
        private readonly Action<string> _sendMessage;
        private readonly Regex _numericRegex;

        public DeathsCommandHandler(BlbApiHandler blbApiHandler, TwitchApiManager twitchApiManager, Action<string> sendMessage)
        {
            _blbApiHandler = blbApiHandler;
            _twitchApiManager = twitchApiManager;
            _sendMessage = sendMessage;
            _numericRegex = new Regex("[0-9]+");
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var message = messageReceivedArgs.ChatMessage.Message;
            var channelInfo = await _twitchApiManager.GetChannelInfo();
            BlbCounter blbCounter;
            if (message.Contains("+"))
            {
                try
                {
                    blbCounter = await _blbApiHandler.IncrementDeathCount(channelInfo.GameId);
                }
                catch (ApiException)
                {
                    SendInvalidDeathCountError(messageReceivedArgs);
                    return;
                }
            }
            else if (message.Contains("-"))
            {
                try
                {
                    blbCounter = await _blbApiHandler.DecrementDeathCount(channelInfo.GameId);
                }
                catch (ApiException)
                {
                    SendInvalidDeathCountError(messageReceivedArgs);
                    return;
                }
            }
            else
            {
                if (!TryExtractNumber(message, out var number))
                    return;

                if (number < 0)
                {
                    SendInvalidDeathCountError(messageReceivedArgs);
                    return;
                }
                blbCounter = await _blbApiHandler.UpdateDeathCount(new BlbCounter()
                {
                    Deaths = number,
                    GameId = channelInfo.GameId
                }, channelInfo.GameId);
            }

            var gameInfo = await _twitchApiManager.GetGameInfo(channelInfo.GameId);
            _sendMessage($"BlackDragon is {blbCounter.Deaths} keer dood gegaan in {gameInfo.Name}");
        }

        private bool TryExtractNumber(string message, out int number)
        {
            var numbers = _numericRegex.Matches(message);
            var firstNumberString = numbers.FirstOrDefault();
            if (firstNumberString is null || !firstNumberString.Success)
            {
                number = -1;
                return false;
            }
            number = int.Parse(firstNumberString.Value);
            return true;
        }

        private void SendInvalidDeathCountError(OnMessageReceivedArgs messageReceivedArgs) => 
            _sendMessage($"{messageReceivedArgs.ChatMessage.Username}, het aantal deaths mag niet onder 0 zijn");
    }
}
