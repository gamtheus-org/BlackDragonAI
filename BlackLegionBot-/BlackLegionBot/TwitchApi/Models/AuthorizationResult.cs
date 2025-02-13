using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BlackLegionBot.TwitchApi.Models
{
    public class AuthorizationResult
    {
        [JsonProperty("access_token")]
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires_in")]
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        [JsonProperty("token_type")]
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

    }
}
