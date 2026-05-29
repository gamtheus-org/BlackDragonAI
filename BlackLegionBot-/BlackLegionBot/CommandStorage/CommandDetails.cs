using MongoDB.Bson;

namespace BlackLegionBot.CommandStorage
{
    public class CommandDetails
    {
        public string _id { get; set; }
        public int Timer { get; set; } = 60;
        public EPermission Permission { get; set; } = EPermission.EVERYONE;
        public string Command { get; set; }
        public string OriginalCommand { get; set; }
        public string Message { get; set; }
    }
}