using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.TwitchApi;

namespace BlackLegionBot.NonCommandBased
{
    public class TimedMessageManager
    {
        private readonly ICommandRetriever _commandRetriever;
        private readonly BlbApiHandler _apiClient;
        private IEnumerable<TimedMessageHandler> _timedMessageHandlers = new TimedMessageHandler[0];
        private readonly Action<string> _sendMessage;
        private LiveStatusManager _liveStatusManager;

        public TimedMessageManager(ICommandRetriever commandRetriever, BlbApiHandler apiClient, Action<string> sendMessage)
        {
            this._commandRetriever = commandRetriever;
            this._apiClient = apiClient;
            this._sendMessage = sendMessage;
        }

        public async Task Start(LiveStatusManager liveStatusManager)
        {
            _liveStatusManager = liveStatusManager;
            RemoveAllActiveTimers();

            var commands = await this._commandRetriever.GetCommands();
            var timedMessages = (await this._apiClient.GetTimedMessages()).ToArray();
            var timedMessageHandlers = new List<TimedMessageHandler>();

            var timeBetweenMessages = 30;
            var totalTimeBetweenMessages = timeBetweenMessages * timedMessages.Count();
            for (var i = 0; i < timedMessages.Count(); i++)
            {
                var tm = timedMessages.ElementAt(i);

                if(commands.TryGetValue(tm.Command, out var command))
                {
                    timedMessageHandlers.Add(new TimedMessageHandler(totalTimeBetweenMessages, 0, 
                        command.Message, _sendMessage, _liveStatusManager, timeBetweenMessages * i));
                }
            }

            this._timedMessageHandlers = timedMessageHandlers;
        }

        private void RemoveAllActiveTimers()
        {
            foreach(var tmh in _timedMessageHandlers)
            {
                tmh.StopTimer();
            }
            _timedMessageHandlers = new TimedMessageHandler[0];
        }

    }
}
