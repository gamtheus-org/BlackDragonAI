using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    public interface ICommandHandler
    {
        Task Handle(OnMessageReceivedArgs messageReceivedArgs);
    }
}