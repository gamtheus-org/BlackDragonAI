using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class GameInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("box_art_url")]
        [JsonPropertyName("box_art_url")]
        public string BoxArtUrl { get; set; }
    }
}
