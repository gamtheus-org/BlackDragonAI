using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class FollowersOperator : ISpecialOperator
    {
        private const string Operator = "$followers";
        private TwitchApiManager TwitchApi { get; }

        public FollowersOperator(TwitchApiManager twitchApi)
        {
            this.TwitchApi = twitchApi;
        }
        
        public string GetOperatorName() => Operator;
        
        public async Task<string> InjectOperatorAsync(string message, string username, string originalCommand)
        {
            if (message.Contains(Operator))
            {
                var followCount = await this.TwitchApi.GetFollowCount();
                message = message.Replace(Operator, followCount.ToString());
            }

            return message;
        }
    }
}