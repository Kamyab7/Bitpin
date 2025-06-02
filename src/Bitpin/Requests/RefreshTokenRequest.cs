using System.Text.Json.Serialization;

namespace Bitpin.Requests;

public class RefreshTokenRequest
{
    [JsonPropertyName("refresh")]
    public string RefreshToken { get; set; } = null!;
}