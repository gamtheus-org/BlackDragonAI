using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class SpecialOperatorHandler
    {
        private readonly Dictionary<string, ISpecialOperator> _specialOperators = new Dictionary<string, ISpecialOperator>();

        public SpecialOperatorHandler(TwitchApiManager twitchApi, ICommandRetriever commandRetriever, BlbApiHandler blbApiHandler)
        {
            var operatorHandlers = new List<ISpecialOperator>()
            {
                new UptimeOperator(twitchApi),
                new UserOperator(),
                new SubsOperator(twitchApi),
                new FollowersOperator(twitchApi),
                new GamesOperator(twitchApi),
                new FollowageOperator(twitchApi),
                new RecipientOperator(),
                new CommandsSpecialOperator(commandRetriever),
                new DeathOperator(blbApiHandler, twitchApi)
            };

            foreach (var operatorHandler in operatorHandlers)
            {
                this._specialOperators.Add(operatorHandler.GetOperatorName(), operatorHandler);
            }
        }

        public async Task<string> InjectOperators(string message, string senderName, string originalMessage)
        {
            if (GetFirstPossibleSpecialOperator(message, out var specialOperator) &&
                this._specialOperators.TryGetValue(specialOperator, out var specialOperatorHandler))
            {
                var messageWithInjectedOperator = await specialOperatorHandler.InjectOperatorAsync(message, senderName, originalMessage);
                return await InjectOperators(messageWithInjectedOperator, senderName, originalMessage);
            }

            return message;
        }

        private bool GetFirstPossibleSpecialOperator(string message, out string specialOperator)
        {
            specialOperator = ExtractOperator(message);
            return !string.IsNullOrEmpty(specialOperator);
        }

        private static string ExtractOperator(string message) =>
            new Regex("[$][a-z]+").Matches(message).FirstOrDefault()?.Value;
    }
}