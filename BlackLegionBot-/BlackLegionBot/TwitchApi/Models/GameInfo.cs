using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class GameInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("box_art_url")]
        public string BoxArtUrl { get; set; }
    }
}
