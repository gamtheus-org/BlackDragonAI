using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class TwitchApiOAuthShareHandler : ICommandHandler
    {
        private readonly TwitchApiManager _twitchApi;
        private readonly Action<string> _sendWhisper;

        public TwitchApiOAuthShareHandler(TwitchApiManager twitchApi, Action<string> sendWhisper)
        {
            _twitchApi = twitchApi;
            _sendWhisper = sendWhisper;
        }

        public Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var oauthToken = _twitchApi.GetAccessToken();
            Console.WriteLine($"Auth token {oauthToken}");
            _sendWhisper($"Access Token: {oauthToken}");
            return Task.CompletedTask;
        }
    }
}
