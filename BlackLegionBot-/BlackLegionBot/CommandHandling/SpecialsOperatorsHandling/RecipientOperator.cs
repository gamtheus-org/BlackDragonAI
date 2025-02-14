using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class RecipientOperator : ISpecialOperator
    {
        private const string Operator = "$recipient";

        public string GetOperatorName() => Operator;

        public async Task<string> InjectOperatorAsync(string message, string username, string originalCommand)
        {
            if (message.Contains(Operator))
            {
                var recipient = originalCommand.ExtractRecipientOfString();
                if (string.IsNullOrEmpty(recipient))
                {
                    return "A name needs to be given for this command";
                }

                return message.Replace(Operator, recipient);
            }

            return message;
        }
    }
}
