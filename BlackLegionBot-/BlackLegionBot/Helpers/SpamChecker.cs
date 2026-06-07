using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using BlackLegionBot.TwitchApi.Models;
using TwitchLib.Client.Models;

namespace BlackLegionBot.Helpers;

public class SpamChecker : IMessageValidator
{
    private const string BanReason = "banned for spam";
    // private static readonly string[] BannedTerms = ["streamboo", "nezhna", "sᴛʀeᴀᴍʙᴏᴏ", "sᴛʀeaᴍʙᴏᴏ"];
    private HashSet<string> BannedTerms { get; set; } = [];

    private readonly TwitchApiManager _twitchApiManager;
    private readonly WebhookHandler _webhookHandler;
    private readonly BlbApiHandler _blbApiHandler;

    public SpamChecker(TwitchApiManager twitchApiManager, WebhookHandler webhookHandler, BlbApiHandler blbApiHandler)
    {
        _twitchApiManager = twitchApiManager;
        _webhookHandler = webhookHandler;
        _blbApiHandler = blbApiHandler;

        LoadBannedTermsAsync();

        _webhookHandler.BannedTermsChanged += () => LoadBannedTermsAsync();
    }

    public async Task LoadBannedTermsAsync()
    {
        BannedTerms = new HashSet<string>(await _blbApiHandler.GetBannedTermsAsync());
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