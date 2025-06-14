using System.Text.Json.Serialization;

namespace Bitpin.Requests;

public class CreateMarketOrderRequest
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("side")]
    public string Side { get; set; } = null!;

    [JsonPropertyName("base_amount")]
    public decimal BaseAmount { get; set; }
}