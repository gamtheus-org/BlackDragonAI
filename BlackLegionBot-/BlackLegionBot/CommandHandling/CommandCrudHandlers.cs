using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class CommandCreateHandler : ICommandHandler
    {
        private readonly CommandCrudManager _crudManager;
        public CommandCreateHandler(CommandCrudManager crudManager)
        {
            this._crudManager = crudManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs) =>
            await this._crudManager.CreateCommand(messageReceivedArgs.ChatMessage.Message);
    }

    public class CommandEditHandler : ICommandHandler
    {
        private readonly CommandCrudManager _crudManager;
        public CommandEditHandler(CommandCrudManager crudManager)
        {
            this._crudManager = crudManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs) =>
            await this._crudManager.EditCommand(messageReceivedArgs.ChatMessage.Message);
    }

    public class CommandDeletionHandler : ICommandHandler
    {
        private readonly CommandCrudManager _crudManager;
        public CommandDeletionHandler(CommandCrudManager crudManager)
        {
            this._crudManager = crudManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs) =>
            await this._crudManager.DeleteCommand(messageReceivedArgs.ChatMessage.Message);
    }

    public class CommandAliasCreationHandler : ICommandHandler
    {
        private readonly CommandCrudManager _crudManager;
        public CommandAliasCreationHandler(CommandCrudManager crudManager)
        {
            this._crudManager = crudManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs) =>
            await this._crudManager.AddAlias(messageReceivedArgs.ChatMessage.Message);
    }

    public class CommandAliasDeletionHandler : ICommandHandler
    {
        private readonly CommandCrudManager _crudManager;
        public CommandAliasDeletionHandler(CommandCrudManager crudManager)
        {
            this._crudManager = crudManager;
        }

        public async Task Handle(OnMessageReceivedArgs messageReceivedArgs) =>
            await this._crudManager.DeleteAlias(messageReceivedArgs.ChatMessage.Message);
    }
}
