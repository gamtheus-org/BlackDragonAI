using System;
using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class StreamData
    {
        public string Id { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string Username { get; set; }
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        [JsonPropertyName("viewer_count")]
        public int ViewerCount { get; set; }
        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }
        public string Language { get; set; }
        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}
