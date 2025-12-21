using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class BroadcasterSubscription
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("is_gift")]
        public bool IsGift { get; set; }
        public string Tier { get; set; }
        [JsonPropertyName("plan_name")]
        public string PlanName { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string Username { get; set; }
    }
}
