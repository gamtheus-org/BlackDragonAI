using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using BlackLegionBot.TwitchApi.Models;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class GameSetterHandler : ICommandHandler
    {
        private readonly TwitchApiManager _apiClient;
        private readonly Action<string> _sendMessageToChannel;

        public GameSetterHandler(TwitchApiManager apiClient, Action<string> sendMessageToChannel)
        {
            this._apiClient = apiClient;
            this._sendMessageToChannel = sendMessageToChannel;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            if ("!setgame ".Length >= messageReceivedArgs.ChatMessage.Message.Length) return;
            var gameName = messageReceivedArgs.ChatMessage.Message.Substring("!setgame ".Length).TrimEnd();
            var gameInfo = await this._apiClient.GetGameInfo(null, gameName);
            if (gameInfo == null)
            {
                _sendMessageToChannel("De genoemde game kan niet gevonden worden. Controlleer de schrijfwijze.");
                return;
            }
            var channelInfo = new ChannelInfo()
            {
                GameId = gameInfo.Id
            };
            await this._apiClient.UpdateChannelInfo(channelInfo);
            _sendMessageToChannel($"De ingestelde game is veranderd naar {gameInfo.Name}");
        }
    }
}
