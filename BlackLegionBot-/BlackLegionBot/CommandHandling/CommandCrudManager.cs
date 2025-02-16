using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using Refit;

namespace BlackLegionBot.CommandHandling
{
    public class CommandCrudManager
    {
        public event Action RefreshOfCommandsRequired;
        private readonly Action<string> _sendMessageToChannel;

        private readonly BlbApiHandler _apiHandler;
        public CommandCrudManager(BlbApiHandler apiHandler, Action<string> sendMessageToChannel)
        {
            this._apiHandler = apiHandler;
            this._sendMessageToChannel = sendMessageToChannel;
        }

        public async Task CreateCommand(string message)
        {
            try
            {
                var commandDetails = ConvertMessageToObject(message, CrudType.CREATE);
                if (CommandNameAvailabilityManager.Available(commandDetails.Command))
                {
                    _sendMessageToChannel("The name, which was used, is reserved.");
                    return;
                }
                await this._apiHandler.CreateCommand(commandDetails);
                _sendMessageToChannel($"The command {commandDetails.Command} has been successfully created");
                RefreshOfCommandsRequired?.Invoke();
                CommandNameAvailabilityManager.AddToBlockList(commandDetails.Command);
            }
            catch (ApiException originalException)
            {
                var apiError = originalException.ToBlbApiError();
                _sendMessageToChannel($"Something went wrong with the following message: {apiError.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        public async Task EditCommand(string message)
        {
            try
            {
                var commandDetails = ConvertMessageToObject(message, CrudType.EDIT);
                if (CommandNameAvailabilityManager.Available(commandDetails.Command))
                {
                    _sendMessageToChannel("The name, which was used, is reserved.");
                    return;
                }
                await this._apiHandler.EditCommand(commandDetails);
                _sendMessageToChannel($"The command {commandDetails.Command} has been successfully edited");
                RefreshOfCommandsRequired?.Invoke();
            }
            catch (ApiException originalException)
            {
                var apiError = originalException.ToBlbApiError();
                _sendMessageToChannel($"Something went wrong with the following message: {apiError.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        public async Task AddAlias(string message)
        {
            var matches = new Regex("![a-z0-9]+").Matches(message.ToLower());
            if (matches.Count == 3)
            {
                var commandToMakeAliasOf = matches.Skip(1).First().Value;
                var alias = matches.Last().Value;
                if (CommandNameAvailabilityManager.Available(commandToMakeAliasOf) || CommandNameAvailabilityManager.Available(alias))
                    return;
                try
                {
                    await this._apiHandler.AddAlias(commandToMakeAliasOf.Substring(1), alias);
                    _sendMessageToChannel($"The alias {alias} for the command {commandToMakeAliasOf} has been successfully created");
                    RefreshOfCommandsRequired?.Invoke();
                    CommandNameAvailabilityManager.AddCommandToBlockList(alias);
                }
                catch (ApiException originalException)
                {
                    var apiError = originalException.ToBlbApiError();
                    _sendMessageToChannel($"Something went wrong with the following message: {apiError.Message}");
                }
            }
        }

        public async Task DeleteCommand(string message)
        {
            try
            {
                var commandToDelete = message.Substring(message.LastIndexOf('!') + 1).TrimEnd();
                await this._apiHandler.DeleteCommand(commandToDelete);
                _sendMessageToChannel($"The command {commandToDelete} has been successfully deleted");
                RefreshOfCommandsRequired?.Invoke();
            }
            catch (ApiException originalException)
            {
                var apiError = originalException.ToBlbApiError();
                _sendMessageToChannel($"Something went wrong with the following message: {apiError.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        public async Task DeleteAlias(string message)
        {
            var matches = new Regex("![a-z0-9]+").Matches(message.ToLower());
            if (matches.Count == 2)
            {
                var alias = matches.Last().Value;
                try
                {
                    await this._apiHandler.DeleteAlias(alias.Substring(1));
                    _sendMessageToChannel($"The alias {alias} has been successfully deleted");
                    RefreshOfCommandsRequired?.Invoke();
                }
                catch (ApiException originalException)
                {
                    var apiError = originalException.ToBlbApiError();
                    _sendMessageToChannel($"Something went wrong with the following message: {apiError.Message}");
                }
            }
        }


        public CommandDetails ConvertMessageToObject(string message, CrudType crudType)
        {
            var lengthOfStartText = crudType switch
            {
                CrudType.CREATE => "!new ".Length,
                CrudType.EDIT => "!edit ".Length,
                _ => throw new ArgumentException("Invalid crud type given")
            };

            var commandContent = message.Substring(lengthOfStartText);
            var commandName = commandContent.Substring(0, commandContent.IndexOf(" "));
            var indexOfFirstQuotes = commandContent.IndexOf('\"');
            var indexOfLastQuotes = commandContent.LastIndexOf('\"');
            var commandMessage = commandContent.Substring(indexOfFirstQuotes + 1, indexOfLastQuotes - indexOfFirstQuotes - 1);

            var commandDetails = new CommandDetails()
            {
                Command = commandName.ToLower(),
                Message = commandMessage
            };
            if (crudType == CrudType.CREATE)
            {
                commandDetails.Permission = ExtractPermission(message);
                commandDetails.Timer = ExtractTimer(message);
            }
            return commandDetails;
        }

        private EPermission ExtractPermission(string message)
        {
            if (message.Contains("p/subs", StringComparison.InvariantCultureIgnoreCase)) 
                return EPermission.SUBS;
            else if (message.Contains("p/mods", StringComparison.InvariantCultureIgnoreCase)) 
                return EPermission.MODS;
            else if (message.Contains("p/admins", StringComparison.InvariantCultureIgnoreCase)) 
                return EPermission.ADMIN;
            else 
                return EPermission.EVERYONE;
        }

        private int ExtractTimer(string message)
        {
            var timerPart = new Regex("t/[0-9]+").Match(message);
            if (!timerPart.Success) return 60;
            var cooldownDurationText = timerPart.Value.Substring(2);
            return int.Parse(cooldownDurationText);
        }
    }

    public enum CrudType
    {
        CREATE, EDIT
    }
}
