using System.Threading.Tasks;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public class UserOperator : ISpecialOperator
    {
        private const string Operator = "$user";
        
        public string GetOperatorName() => Operator;

        public Task<string> InjectOperatorAsync(string message, string username, string originalMessage) =>
            Task.FromResult(message.Replace(Operator, username));
    }
}