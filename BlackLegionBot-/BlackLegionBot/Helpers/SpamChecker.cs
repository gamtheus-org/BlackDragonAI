using System;
using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.NonCommandBased;
using TwitchLib.Client.Models;

namespace BlackLegionBot.Helpers;

public class SpamChecker : IMessageValidator
{
    private static readonly string[] BannedTerms = ["streamboo"];

    private readonly Func<string, Task> _sendMessageToChannelAsync;

    public SpamChecker(Func<string, Task> sendMessageToChannelAsync)
    {
        _sendMessageToChannelAsync = sendMessageToChannelAsync;
    }

    public bool Validate(ChatMessage chatMessage)
    {
        Console.WriteLine("Checking message for banned term");
        var messageContainsBannedTerm = BannedTerms.Any(bannedTerm => chatMessage.Message.Contains(bannedTerm, StringComparison.InvariantCultureIgnoreCase));
        if (messageContainsBannedTerm)
        {
            Console.WriteLine("Message contains a banned term");
        }
        return !messageContainsBannedTerm;
    }

    public async Task HandleValidationErrorAsync(ChatMessage chatMessage)
    {
        Console.WriteLine("Banning user for sending a message with a banned term: " + chatMessage.Message);
        await _sendMessageToChannelAsync($"/ban {chatMessage.Username}");
    }
}