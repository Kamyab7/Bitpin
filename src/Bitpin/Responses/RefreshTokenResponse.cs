using System.Text.Json.Serialization;

namespace Bitpin.Responses;

public class RefreshTokenResponse
{
    [JsonPropertyName("access")]
    public string AccessToken { get; set; } = null!;
}