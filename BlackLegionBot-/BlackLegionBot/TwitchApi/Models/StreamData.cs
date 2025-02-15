using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class StreamData
    {
        public string Id { get; set; }
        [JsonProperty("user_id")]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonProperty("user_name")]
        [JsonPropertyName("user_name")]
        public string Username { get; set; }
        [JsonProperty("game_id")]
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        [JsonProperty("viewer_count")]
        [JsonPropertyName("viewer_count")]
        public int ViewerCount { get; set; }
        [JsonProperty("started_at")]
        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }
        public string Language { get; set; }
        [JsonProperty("thumbnail_url")]
        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}
