using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackLegionBot.NonCommandBased;
using Refit;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class CommercialStarterHandler : ICommandHandler
    {
        private readonly CommercialManager _commercialManager;
        private readonly Func<string, Task> _sendMessageToChannelAsync;

        public CommercialStarterHandler(CommercialManager commercialManager, Func<string, Task> sendMessageToChannelAsync)
        {
            this._commercialManager = commercialManager;
            this._sendMessageToChannelAsync = sendMessageToChannelAsync;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var lengthString = new Regex("[0-9]+").Matches(messageReceivedArgs.ChatMessage.Message).FirstOrDefault()?.Value;
            ECommercialLength length = ECommercialLength.L30;
            if (!string.IsNullOrEmpty(lengthString))
            {
                try
                {
                    length = (ECommercialLength) int.Parse(lengthString);
                    if(!Enum.IsDefined(typeof(ECommercialLength), length))
                        throw new InvalidCastException("Invalid value");
                }
                catch (InvalidCastException)
                {
                    await _sendMessageToChannelAsync("Invalid length. The length of an advertisement can only be: 30, 60, 90, 120, 150 or 180");
                    return;
                }
            }

            try
            {
                await this._commercialManager.StartCommercial(length);
                await _sendMessageToChannelAsync($"An ad of {length.ToString().Substring(1)} seconds has been started");
            }
            catch(ApiException)
            {
                await _sendMessageToChannelAsync("Something went wrong with trying to start the advertisement. The most likely cause is the stream bot being live.");
            }
        }
    }
}
