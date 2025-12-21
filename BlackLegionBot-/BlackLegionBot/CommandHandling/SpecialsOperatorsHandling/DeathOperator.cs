using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class DeathOperator : ISpecialOperator
    {
        private const string Operator = "$deaths";
        private readonly BlbApiHandler _blbApiHandler;
        private readonly TwitchApiManager _twitchApiManager;

        public DeathOperator(BlbApiHandler blbApiHandler, TwitchApiManager twitchApiManager)
        {
            _blbApiHandler = blbApiHandler;
            _twitchApiManager = twitchApiManager;
        }

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalMessage)
        {
            var channelInfo = await _twitchApiManager.GetChannelInfo();
            var deaths = await GetDeaths(channelInfo.GameId);
            return $"BlackDragon has died {deaths} times in {channelInfo.GameName}";
        }

        private async Task<int> GetDeaths(string gameId)
        {
            var deathCount = await _blbApiHandler.GetDeathCount(gameId);
            return deathCount?.Deaths ?? 0;
        }
    }
}
