using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandHandling;

namespace BlackLegionBot.CommandStorage
{
    public static class CommandNameAvailabilityManager
    {
        private static readonly HashSet<string> BlockList = new HashSet<string>()
        {
            "!new", "!delete", "!edit", "!setalias", "!setgame", "!auth", "!deletealias", "settitle"
        };
        private static readonly HashSet<string> UsedCommandsSet = new HashSet<string>();

        public static void RemoveFromBlockListBulk(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                BlockList.Remove(name);
            }
        }

        public static bool Available(string name) => 
            BlockList.Contains(name.ToLower()) && UsedCommandsSet.Contains(name.ToLower());

        // General
        public static void AddToBlockList(string name) =>
            BlockList.Add(name);

        public static void AddAllToBlockListBulk(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                AddToBlockList(name.ToLower());
            }
        }

        public static void RemoveFromBlockList(string name) => 
            BlockList.Remove(name);


        // Commands
        public static void AddCommandToBlockList(string commandName) =>
            UsedCommandsSet.Add(commandName.ToLower());

        public static void AddCommandsToBlockList(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                AddCommandToBlockList(name);
            }
        }

        public static void RemoveCommandFromBlockList(string name) =>
            UsedCommandsSet.Remove(name.ToLower());

        public static void RemoveCommandsFromBlockList(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                RemoveCommandFromBlockList(name);
            }
        }
    }
}
