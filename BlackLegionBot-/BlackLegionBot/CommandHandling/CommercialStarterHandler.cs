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
        private readonly Action<string> _sendMessageToChannel;

        public CommercialStarterHandler(CommercialManager commercialManager, Action<string> sendMessageToChannel)
        {
            this._commercialManager = commercialManager;
            this._sendMessageToChannel = sendMessageToChannel;
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
                    _sendMessageToChannel("Invalide length. De lengte van een advertentie kan zijn: 30, 60, 90, 120, 150 en 180");
                    return;
                }
            }

            try
            {
                await this._commercialManager.StartCommercial(length);
                _sendMessageToChannel($"Een advertentie van {length.ToString().Substring(1)} seconden is gestart");
            }
            catch(ApiException)
            {
                _sendMessageToChannel("Er is iets mis gegaan met het starten van de advertentie. Waarschijnlijk is de stream momenteel niet live.");
            }
        }
    }
}
