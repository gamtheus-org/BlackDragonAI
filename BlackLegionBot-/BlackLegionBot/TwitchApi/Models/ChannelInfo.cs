using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BlackLegionBot.TwitchApi.Models
{
    public class ChannelInfo
    {
        [JsonProperty("broadcaster_id")]
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonProperty("broadcaster_name")]
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonProperty("broadcaster_language")]
        [JsonPropertyName("broadcaster_language")]
        public string BroadCasterLanguage { get; set; }
        [JsonProperty("game_id")]
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonProperty("game_name")]
        [JsonPropertyName("game_name")]
        public string GameName { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}
