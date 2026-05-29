using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class FollowageOperator : ISpecialOperator
    {
        private const string Operator = "$followage";

        private readonly TwitchApiManager TwitchApi;
        public FollowageOperator(TwitchApiManager apiClient)
        {
            this.TwitchApi = apiClient;
        }

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalCommand)
        {
            if (message.Contains(Operator))
            {
                var userInfo = await this.TwitchApi.GetUserDetails(username);
                var followInfo = await this.TwitchApi.GetFollowerInfo(userInfo.Id);
                message = message.Replace(Operator, followInfo.FollowedAt.ConvertToDifferenceFromNowInEnglish(TimeSpanConversionLimit.DAYS, TimeSpanConversionLimit.YEARS));
            }

            return message;
        }
    }
}
