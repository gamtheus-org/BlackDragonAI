using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class FollowerInfo
    {
        [JsonProperty("user_id")]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_name")]
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_login")]
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }

        [JsonProperty("followed_at")]
        [JsonPropertyName("followed_at")]
        public DateTime FollowedAt { get; set; }
    }
}
