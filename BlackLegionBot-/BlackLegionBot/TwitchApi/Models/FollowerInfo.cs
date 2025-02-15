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
        [JsonProperty("from_id")]
        [JsonPropertyName("from_id")]
        public string FromId { get; set; }

        [JsonProperty("from_name")]
        [JsonPropertyName("from_name")]
        public string FromName { get; set; }

        [JsonProperty("to_id")]
        [JsonPropertyName("to_id")]
        public string ToId { get; set; }

        [JsonProperty("to_name")]
        [JsonPropertyName("to_name")]
        public string ToName { get; set; }

        [JsonProperty("followed_at")]
        [JsonPropertyName("followed_at")]
        public DateTime FollowedAt { get; set; }
    }
}
