using BlackLegionBot.CommandStorage;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class CommandSelector
    {
        private Bot Bot { get; }
        private TwitchApiManager TwitchApi { get; }
        private readonly IMessageValidator[] _messageValidators;

        private GenericCommandHandler GenericCommandHandler { get; }
        private readonly AuthCommandHandler _authCommandHandler;
        private readonly CommandCreateHandler _commandCreateHandler;
        private readonly CommandEditHandler _commandEditHandler;
        private readonly CommandDeletionHandler _commandDeletionHandler;
        private readonly GameSetterHandler _gameSetterHandler;
        private readonly TitleSetterHandler _titleSetterHandler;
        private readonly CommandAliasCreationHandler _commandAliasCreationHandler;
        private readonly CommandAliasDeletionHandler _commandAliasDeletionHandler;
        private readonly CommercialStarterHandler _commercialStarterHandler;
        private readonly PermissionHandler _permissionHandler;
        private readonly DeathsCommandHandler _deathsCommandHandler;
        private readonly TwitchApiOAuthShareHandler _twitchApiOAuthSharer;
        private readonly CounterCreationCommandHandler _counterCreationCommandHandler;
        private readonly CounterDeletionCommandHandler _counterDeletionCommandHandler;
        private readonly CounterRetrievalCommandHandler _counterRetrievalCommandHandler;

        public CommandSelector(Bot bot, TwitchApiManager twitchApi, ICommandRetriever commandRetriever,
            BlbApiHandler blbApiClient, CooldownManager cooldownManager, CommercialManager commercialManager)
        {
            this.Bot = bot;
            this.TwitchApi = twitchApi;
            var urlChecker = new UrlChecker(bot);
            var capsChecker = new CapsChecker(bot);
            _messageValidators = new IMessageValidator[] { urlChecker, capsChecker };

            this.GenericCommandHandler = new GenericCommandHandler(commandRetriever, Bot, TwitchApi, cooldownManager, blbApiClient);
            this._authCommandHandler = new AuthCommandHandler(twitchApi);
            var crudManager = new CommandCrudManager(blbApiClient, Bot.SendMessageToChannel);
            crudManager.RefreshOfCommandsRequired += commandRetriever.RetrieveCommands;
            this._commandCreateHandler = new CommandCreateHandler(crudManager);
            this._commandEditHandler = new CommandEditHandler(crudManager);
            this._commandDeletionHandler = new CommandDeletionHandler(crudManager);
            this._commandAliasCreationHandler = new CommandAliasCreationHandler(crudManager);
            this._commandAliasDeletionHandler = new CommandAliasDeletionHandler(crudManager);
            this._gameSetterHandler = new GameSetterHandler(twitchApi, Bot.SendMessageToChannel);
            this._titleSetterHandler = new TitleSetterHandler(twitchApi, Bot.SendMessageToChannel);
            this._commercialStarterHandler = new CommercialStarterHandler(commercialManager, Bot.SendMessageToChannel);
            this._permissionHandler = new PermissionHandler(urlChecker, bot);
            this._deathsCommandHandler = new DeathsCommandHandler(blbApiClient, twitchApi, Bot.SendMessageToChannel);
            this._twitchApiOAuthSharer = new TwitchApiOAuthShareHandler(twitchApi, mesg => Bot.SendWhisperToChannel(mesg, "gamtheus"));
            this._counterCreationCommandHandler = new CounterCreationCommandHandler(blbApiClient, bot);
            this._counterDeletionCommandHandler = new CounterDeletionCommandHandler(blbApiClient, bot);
            this._counterRetrievalCommandHandler = new CounterRetrievalCommandHandler(blbApiClient, bot);
            this._counterCreationCommandHandler.OnCounterCreated += this._counterRetrievalCommandHandler.AddCounter;
            this._counterDeletionCommandHandler.OnCounterDeleted += this._counterRetrievalCommandHandler.DeleteCounter;
        }

        public async Task HandleCommand(object sender, OnMessageReceivedArgs messageReceivedArgs)
        {
            if (!EPermission.MODS.HasEqualOrHigherPermission(messageReceivedArgs.ChatMessage.GetPermissionOfSender()))
            {
                var failedValidator = this._messageValidators.FirstOrDefault(mv => !mv.Validate(messageReceivedArgs.ChatMessage));
                if (failedValidator != null)
                {
                    failedValidator.HandleValidationError(messageReceivedArgs.ChatMessage);
                    return;
                }
            }

            try
            {
                foreach (var commandHandler in GetCommandHandlers(messageReceivedArgs))
                {
                    await commandHandler.Handle(messageReceivedArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!");
                Console.WriteLine($"Error: {e.Message}");
                this.Bot.SendWhisperToChannel($"Message: {e.Message} \nStackTrace: {e.StackTrace}", "gamtheus");
            }
        }

        private IEnumerable<ICommandHandler> GetCommandHandlers(OnMessageReceivedArgs onMessageReceivedArgs)
        {
            var senderIsAdmin = onMessageReceivedArgs.ChatMessage.IsAdmin();
            var senderIsModOrHigher = EPermission.MODS.HasEqualOrHigherPermission(onMessageReceivedArgs.ChatMessage.GetPermissionOfSender());
            var senderIsSubOrHigher = EPermission.SUBS.HasEqualOrHigherPermission(onMessageReceivedArgs.ChatMessage.GetPermissionOfSender());
            var calledCommand = onMessageReceivedArgs.ChatMessage.GetCalledCommand();
            return calledCommand switch
            {
                "!crash" when senderIsAdmin => new[] { new CrashCommandHandler() },
                "!oauth" when senderIsAdmin => new[] { this._twitchApiOAuthSharer },
                "!auth" when senderIsAdmin => new[] { this._authCommandHandler },
                "!new" when senderIsModOrHigher => new[] { this._commandCreateHandler },
                "!edit" when senderIsModOrHigher => new[] { this._commandEditHandler },
                "!setalias" when senderIsModOrHigher => new[] { this._commandAliasCreationHandler },
                "!addalias" when senderIsModOrHigher => new[] { this._commandAliasCreationHandler },
                "!deletealias" when senderIsModOrHigher => new[] { this._commandAliasDeletionHandler },
                "!delete" when senderIsModOrHigher => new[] { this._commandDeletionHandler },
                "!setgame" when senderIsModOrHigher => new[] { this._gameSetterHandler },
                "!settitle" when senderIsModOrHigher => new[] { this._titleSetterHandler },
                "!startcommercial" when senderIsModOrHigher => new[] { this._commercialStarterHandler },
                "!permit" when senderIsModOrHigher => new[] { this._permissionHandler },
                "!newcounter" when senderIsModOrHigher => new[] { this._counterCreationCommandHandler },
                "!deletecounter" when senderIsModOrHigher => new[] { this._counterDeletionCommandHandler },
                "!deaths" when senderIsSubOrHigher &&
                               calledCommand.Length != onMessageReceivedArgs.ChatMessage.Message.Length =>
                new[] { this._deathsCommandHandler },
                _ => new[] { (ICommandHandler)GenericCommandHandler, this._counterRetrievalCommandHandler }
            };
        }
    }

    public class CrashCommandHandler : ICommandHandler
    {
        public Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            throw new Exception("Bot deed krak");
        }
    }
}