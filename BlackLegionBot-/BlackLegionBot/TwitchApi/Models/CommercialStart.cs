using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class CommercialStartInput
    {
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
