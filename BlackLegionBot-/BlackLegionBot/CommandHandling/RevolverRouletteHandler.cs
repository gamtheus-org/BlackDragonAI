using System;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling;

// Kata: If you win, nothing happens just says you won for 1 time, or 2, or whatever and if you lose you get a 10 seconds ban
public class RevolverRouletteHandler : ICommandHandler
{
    private const int NumberOfSecondsToTimeout = 10;

    private readonly Func<string, Task> _sendMessageAsync;
    private readonly Func<string, TimeSpan, Task> _timeoutUserAsync;

    public RevolverRouletteHandler(Func<string, Task> sendMessageAsync, Func<string, TimeSpan, Task> timeoutUserAsync)
    {
        _sendMessageAsync = sendMessageAsync;
        _timeoutUserAsync = timeoutUserAsync;
    }
    
    public async Task Handle(OnMessageReceivedArgs messageReceivedArgs)
    {
        var survives = Random.Shared.Next(2) == 0;
        if (survives)
        {
            await _sendMessageAsync($"{messageReceivedArgs.ChatMessage.Username} you have survived the roulette!");
        }
        else
        {
            await _sendMessageAsync($"{messageReceivedArgs.ChatMessage.Username}'s revolver went bang.");
            await _timeoutUserAsync(messageReceivedArgs.ChatMessage.Username, TimeSpan.FromSeconds(NumberOfSecondsToTimeout));
        }
    }
}