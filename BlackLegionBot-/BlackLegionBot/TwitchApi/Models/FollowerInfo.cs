using System;
using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class FollowerInfo
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }

        [JsonPropertyName("followed_at")]
        public DateTime FollowedAt { get; set; }
    }
}
