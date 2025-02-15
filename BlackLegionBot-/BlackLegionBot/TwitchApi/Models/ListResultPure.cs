using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class ListResultPure<T>
    {
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; }
    }
}
