using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Refit;

namespace BlackLegionBot.CommandStorage
{
    public class CommandCache : ICommandRetriever
    {
        private Task<Dictionary<string, CommandDetails>> _commandsTask;
        private DateTime _dateOfRequiredRefreshUtc;
        private readonly BlbApiHandler _blbApi;
        
        public CommandCache(BlbApiHandler blbApi)
        {
            _blbApi = blbApi;
        }

        public async Task<(bool isFound, CommandDetails command)> TryGetCommand(string name)
        {
            var commands = await GetCommands();
            var isFound = commands.TryGetValue(name, out var command);
            return (isFound, command);
        }

        public void RetrieveCommands()
        {
            _dateOfRequiredRefreshUtc = DateTime.UtcNow.Add(TimeSpan.FromDays(1));
            _commandsTask = GetDictionaryOfCommands(_blbApi.GetAllCommands);
        }

        public async Task<Dictionary<string, CommandDetails>> GetCommands()
        {
            if (_commandsTask is null || _dateOfRequiredRefreshUtc < DateTime.UtcNow)
            {
                RetrieveCommands();
            }
            
            try
            {
                return await _commandsTask;
            }
            catch (ApiException)
            {
                // If api call fails, reset _commandTask and redo the call
                _commandsTask = null;
                return await GetCommands();
            }
        }

        private static async Task<Dictionary<string, CommandDetails>> GetDictionaryOfCommands(
            Func<Task<IEnumerable<CommandDetails>>> retrieveCommands)
        {
            var commands = await retrieveCommands();
            return commands.ToDictionary(command => command.Command);
        }
    }
}