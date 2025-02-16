using System;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using Newtonsoft.Json;
using TwitchLib.Api.Interfaces;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class GamesOperator : ISpecialOperator
    {
        private const string Operator = "$game";
        private TwitchApiManager TwitchApi { get; }

        public GamesOperator(TwitchApiManager twitchApi)
        {
            this.TwitchApi = twitchApi;
        }
        
        public string GetOperatorName() => Operator;
        
        public async Task<string> InjectOperatorAsync(string message, string username, string originalCommand)
        {
            if (message.Contains(Operator))
            {
//                var streamData = await this.TwitchApi.GetStreamData();
//                var gameInfo = await this.TwitchApi.GetGameInfo(streamData.GameId);
                var channelInfo = await this.TwitchApi.GetChannelInfo();
                message = message.Replace(Operator, channelInfo.GameName);
            }

            return message;
        }
    }
}