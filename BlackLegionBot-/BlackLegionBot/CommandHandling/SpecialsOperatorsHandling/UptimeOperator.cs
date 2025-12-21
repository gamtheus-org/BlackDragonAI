using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class UptimeOperator : ISpecialOperator
    {
        private const string Operator = "$uptime";
        private TwitchApiManager TwitchApi { get; }

        public UptimeOperator(TwitchApiManager twitchApi)
        {
            this.TwitchApi = twitchApi;
        }

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalMessage)
        {
            if (message.Contains(Operator))
            {
                var streamData = await this.TwitchApi.GetStreamData();
                if (streamData == null)
                {
                    return "The stream is sadly not live. Try again when the stream is live.";
                }

                message = message.Replace(Operator, streamData.StartedAt.ConvertToDifferenceFromNowInEnglish(TimeSpanConversionLimit.SECONDS, TimeSpanConversionLimit.HOURS));
            }

            return message;
        }
    }
}