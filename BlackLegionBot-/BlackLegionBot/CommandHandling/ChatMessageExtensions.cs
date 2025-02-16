using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BlackLegionBot.CommandStorage;
using TwitchLib.Client.Models;

namespace BlackLegionBot.CommandHandling
{
    public static class ChatMessageExtensions
    {
        public static bool IsAdmin(this ChatMessage chatMessage) =>
            Bot.NamesOfAdmins.Any(name =>
                name.Equals(chatMessage.Username, StringComparison.InvariantCultureIgnoreCase));

        public static EPermission GetPermissionOfSender(this ChatMessage chatMessage)
        {
            if (chatMessage.IsAdmin())
                return EPermission.ADMIN;
            else if (chatMessage.IsModerator)
                return EPermission.MODS;
            else if (chatMessage.IsSubscriber)
                return EPermission.MODS;
            else
                return EPermission.EVERYONE;
        }

        public static string GetCalledCommand(this ChatMessage chatMessage)
        {
            var originalMessage = chatMessage.Message;
            var indexOfWhiteSpace = originalMessage.IndexOf(' ');
            var lengthOfCommand = indexOfWhiteSpace >= 0 ? indexOfWhiteSpace : originalMessage.Length;
            return originalMessage.Substring(0, lengthOfCommand);
        }
        
        public static string ExtractRecipient(this ChatMessage chatMessage) =>
            CommonRegexes.UsernameRegex.Matches(chatMessage.Message).Skip(1).FirstOrDefault()?.Value;

        public static string ExtractRecipientOfString(this string message) =>
            CommonRegexes.UsernameRegex.Matches(message).Skip(1).FirstOrDefault()?.Value;
    }
}