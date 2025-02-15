using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BlackLegionBot.TwitchApi.Models
{
    public class ListResultPure<T>
    {
        [JsonPropertyName("data")]
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }
    }
}
