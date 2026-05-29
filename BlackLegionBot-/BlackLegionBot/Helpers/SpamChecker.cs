using System;
using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using BlackLegionBot.TwitchApi.Models;
using TwitchLib.Client.Models;

namespace BlackLegionBot.Helpers;

public class SpamChecker : IMessageValidator
{
    private const string BanReason = "banned for spam";
    private static readonly string[] BannedTerms = ["streamboo", "nezhna", "sᴛʀeᴀᴍʙᴏᴏ", "sᴛʀeaᴍʙᴏᴏ"];

    private readonly TwitchApiManager _twitchApiManager;

    public SpamChecker(TwitchApiManager twitchApiManager)
    {
        _twitchApiManager = twitchApiManager;
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
        Console.WriteLine($"Banning user ({chatMessage.UserId}) for sending a message with a banned term");
        var banInfo = new BanUserInput()
        {
            UserId = chatMessage.UserId,
            Reason = BanReason
        };
        await _twitchApiManager.BanUserAsync(banInfo);
    }
}