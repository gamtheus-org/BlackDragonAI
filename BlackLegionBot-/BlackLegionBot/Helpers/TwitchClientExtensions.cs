using System;
using System.Threading.Tasks;
using TwitchLib.Client;

namespace BlackLegionBot.Helpers;

public static class TwitchClientExtensions
{
    public static async Task TimeoutUserAsync(this TwitchClient client, string channelName, string username, TimeSpan duration)
    {
        var command = $"/timeout {username} {duration.TotalSeconds}";
        await client.SendMessageAsync(channelName, command);
    }
}