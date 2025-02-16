using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi;
using TwitchLib.Client.Events;

namespace BlackLegionBot.CommandHandling
{
    class AuthCommandHandler : ICommandHandler
    {
        private readonly TwitchApiManager _apiClient;
        private Task _reauthTask;

        public AuthCommandHandler(TwitchApiManager apiClient)
        {
            this._apiClient = apiClient;
        }

        public Task Handle(OnMessageReceivedArgs messageReceivedArgs)
        {
            if (_reauthTask == null)
            {
                _reauthTask = this._apiClient.AuthManager.Reauthorize();
            }

            return Task.CompletedTask;
        }
    }
}
