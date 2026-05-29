using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace BlackLegionBot.NonCommandBased
{
    public interface IMessageValidator
    {
        bool Validate(ChatMessage chatMessage);

        Task HandleValidationErrorAsync(ChatMessage chatMessage);
    }
}
