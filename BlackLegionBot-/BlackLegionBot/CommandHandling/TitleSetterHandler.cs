using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using BlackLegionBot.TwitchApi.Models;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    class TitleSetterHandler : ICommandHandler
    {
        private readonly TwitchApiManager _apiClient;
        private readonly Action<string> _sendMessageToChannel;

        public TitleSetterHandler(TwitchApiManager apiClient, Action<string> sendMessageToChannel)
        {
            this._apiClient = apiClient;
            this._sendMessageToChannel = sendMessageToChannel;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            if ("!settitle".Length >= messageReceivedArgs.ChatMessage.Message.Length) return;
            
            var title = messageReceivedArgs.ChatMessage.Message.Substring("!settitle ".Length).TrimEnd();
            var channelInfo = new ChannelInfo()
            {
                Title = title
            };

            try
            {
                await this._apiClient.UpdateChannelInfo(channelInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(e.Message);
                return;
            }
            
            _sendMessageToChannel($"De ingestelde titel is veranderd naar {title}");
        }
    }
}
