using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using TwitchLib.Api.Interfaces;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class SubsOperator : ISpecialOperator
    {
        private const string Operator = "$subs";
        private TwitchApiManager TwitchApi { get; }

        public SubsOperator(TwitchApiManager twitchApi)
        {
            this.TwitchApi = twitchApi;
        }

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalMessage)
        {
            if (message.Contains(Operator))
            {
                var subs = await this.TwitchApi.GetSubscriptions();

                message = message.Replace(Operator, subs.Count().ToString());
            }

            return message;
        }
    }
}