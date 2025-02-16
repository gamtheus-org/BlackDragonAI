using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using BlackLegionBot.CommandHandling;
using BlackLegionBot.CommandStorage;
using TwitchLib.Client.Models;

namespace BlackLegionBot.NonCommandBased
{
    public class UrlChecker : IMessageValidator
    {
        private readonly Bot _bot;
        private readonly HashSet<string> _peopleWithPermissionToSendMessageWithUrl = new HashSet<string>();

        public UrlChecker(Bot bot)
        {
            this._bot = bot;
        }

        public bool Validate(ChatMessage chatMessage) =>
            !ContainsUrl(chatMessage.Message) || 
            EPermission.SUBS.HasEqualOrHigherPermission(chatMessage.GetPermissionOfSender()) || 
            _peopleWithPermissionToSendMessageWithUrl.Remove(chatMessage.Username.ToLower());

        public void HandleValidationError(ChatMessage chatMessage) =>
            this._bot.TimeoutUser(chatMessage.Username, 1, "Usage of URIs is not allowed");

        private static bool ContainsUrl(string message) =>
            new Regex(".*([.]?[-a-zA-Z0-9\\\\@:%_+~#=]|)+([-a-zA-Z0-9\\\\@:%_+~#=]){1,254}[.][a-zA-z0-9]{2,6}.*").IsMatch(message);


        public bool AddPermission(string username) =>
            _peopleWithPermissionToSendMessageWithUrl.Add(username.ToLower());
    }
}
