using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class UserDetails
    {
        public string Id { get; set; }
        public string Login { get; set; }
        [JsonProperty("display_name")]
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        public string Type { get; set; }
        [JsonProperty("broadcaster_type")]
        [JsonPropertyName("broadcaster_type")]
        public string BroadcasterType{ get; set; }
        public string Description { get; set; }
        [JsonProperty("profile_image_url")]
        [JsonPropertyName("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        [JsonProperty("offline_image_url")]
        [JsonPropertyName("offline_image_url")]
        public string OfflineImageUrl { get; set; }
        [JsonProperty("view_count")]
        [JsonPropertyName("view_count")]
        public int ViewCount { get; set; }
        public string Email { get; set; }
    }
}
