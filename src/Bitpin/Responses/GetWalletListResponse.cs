using System.Text.Json.Serialization;

namespace Bitpin.Responses;

public class GetWalletListResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("asset")]
    public string Asset { get; set; } = null!;

    [JsonPropertyName("balance")]
    public string Balance { get; set; } = null!;

    [JsonPropertyName("frozen")]
    public string Frozen { get; set; } = null!;

    [JsonPropertyName("service")]
    public string Service { get; set; } = null!;
}