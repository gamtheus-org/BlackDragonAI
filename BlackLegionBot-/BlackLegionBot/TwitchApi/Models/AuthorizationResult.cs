using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models
{
    public class AuthorizationResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

    }
}
