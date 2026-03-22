using System;
using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.NonCommandBased;
using TwitchLib.Client.Models;

namespace BlackLegionBot.Helpers;

public class SpamChecker : IMessageValidator
{
    private static readonly string[] BannedTerms = ["streamboo .com", "streamboo .live"];

    private readonly Func<string, Task> _sendMessageToChannelAsync;

    public SpamChecker(Func<string, Task> sendMessageToChannelAsync)
    {
        _sendMessageToChannelAsync = sendMessageToChannelAsync;
    }

    public bool Validate(ChatMessage chatMessage)
    {
        var messageContainsBannedTerm = BannedTerms.Any(chatMessage.Message.Contains);
        return !messageContainsBannedTerm;
    }

    public async Task HandleValidationErrorAsync(ChatMessage chatMessage)
    {
        await _sendMessageToChannelAsync($"/ban {chatMessage.Username}");
    }
}