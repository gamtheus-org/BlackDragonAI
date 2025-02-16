using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class CommandsSpecialOperator : ISpecialOperator
    {
        private const string Operator = "$commands";
        private readonly ICommandRetriever _commandRetriever;

        public CommandsSpecialOperator(ICommandRetriever commandRetriever)
        {
            this._commandRetriever = commandRetriever;
        }

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalMessage)
        {
            var commands = await this._commandRetriever.GetCommands();
            var commandNames = commands.Values.Where(c => c.Permission >= EPermission.SUBS)
                .Select(c => c.OriginalCommand).Distinct();
            return message.Replace(Operator, commandNames.Aggregate((c1, c2) => $"{c1}, {c2}")).ReplaceLastOccurrence(",", " en");
        }
    }
}
