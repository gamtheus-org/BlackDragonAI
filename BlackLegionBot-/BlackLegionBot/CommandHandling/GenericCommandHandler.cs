using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.CommandHandling.SpecialsOperatorsHandling;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class GenericCommandHandler : ICommandHandler
    {
        private ICommandRetriever CommandRetriever { get; }
        private Bot Bot { get; }
        private readonly SpecialOperatorHandler _operatorHandler;
        private readonly CooldownManager _cooldownManager;

        public GenericCommandHandler(ICommandRetriever commandRetriever, Bot bot, TwitchApiManager twitchAPi, CooldownManager cooldownManager, BlbApiHandler blbApiHandler)
        {
            this.CommandRetriever = commandRetriever;
            this.Bot = bot;
            this._operatorHandler = new SpecialOperatorHandler(twitchAPi, commandRetriever, blbApiHandler);
            this._cooldownManager = cooldownManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var chatMessage = messageReceivedArgs.ChatMessage;
            var calledCommand = chatMessage.GetCalledCommand();
            var (commandIsFound, command) = await CommandRetriever.TryGetCommand(calledCommand);
            if (commandIsFound && command.Permission >= chatMessage.GetPermissionOfSender() && 
                (!_cooldownManager.IsInCooldown(command.OriginalCommand) || EPermission.MODS >= chatMessage.GetPermissionOfSender()))
            {
                _cooldownManager.StartCooldown(command.OriginalCommand);
                var messageToSend = await _operatorHandler.InjectOperators(command.Message, chatMessage.Username, messageReceivedArgs.ChatMessage.Message);
                Bot.SendMessageToChannelAsync(messageToSend);
            }
        }
    }
}