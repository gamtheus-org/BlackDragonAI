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
            Console.WriteLine($"Injecting {Operator}");
            if (message.Contains(Operator))
            {
                Console.WriteLine($"contains {Operator}");
//                var streamData = await this.TwitchApi.GetStreamData();
//                var gameInfo = await this.TwitchApi.GetGameInfo(streamData.GameId);
                var channelInfo = await this.TwitchApi.GetChannelInfo();
                Console.WriteLine(JsonConvert.SerializeObject(channelInfo));
                message = message.Replace(Operator, channelInfo.GameName);
            }

            return message;
        }
    }
}