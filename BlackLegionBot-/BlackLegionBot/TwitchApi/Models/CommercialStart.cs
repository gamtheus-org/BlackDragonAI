using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class CommercialStartInput
    {
        [JsonProperty("broadcaster_id")]
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        public int Length { get; set; }
    }

    public class CommercialStartOutput
    {
        public int Length { get; set; }
        public string Message { get; set; }
        public int RetryAfter { get; set; }
    }
}
