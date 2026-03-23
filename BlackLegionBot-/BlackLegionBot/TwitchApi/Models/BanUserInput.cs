using System.Text.Json.Serialization;

namespace BlackLegionBot.TwitchApi.Models;

public record BanUserInput
{
    [JsonPropertyName("user_id")]
    public string UserId { get; init; }

    [JsonPropertyName("readon")]
    public string Reason { get; init; }
}

public record BanUserInputWrapper
{
    public BanUserInput Data { get; init; }
}