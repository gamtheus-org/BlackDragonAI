using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class ChannelInfo
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("broadcaster_language")]
        public string BroadCasterLanguage { get; set; }
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("game_name")]
        public string GameName { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}
