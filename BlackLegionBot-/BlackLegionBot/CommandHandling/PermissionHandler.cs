using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.NonCommandBased;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public class PermissionHandler : ICommandHandler
    {
        private readonly UrlChecker _urlChecker;
        private readonly Bot _bot;

        public PermissionHandler(UrlChecker urlChecker, Bot bot)
        {
            this._urlChecker = urlChecker;
            this._bot = bot;
        }

        public Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            var recipient = messageReceivedArgs.ChatMessage.ExtractRecipient();
            var addedPermission = this._urlChecker.AddPermission(recipient);
            if (addedPermission)
            {
                this._bot.SendMessageToChannel($"@{recipient}, Je hebt de toestemming gekregen om eenmalig een bericht met een link er in te sturen");
            }
            else
            {
                this._bot.SendMessageToChannel($"@{messageReceivedArgs.ChatMessage.DisplayName}, De gebruiker heeft al permissie gekregen");
            }

            return Task.CompletedTask;
        }
    }
}
