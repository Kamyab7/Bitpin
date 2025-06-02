using System.Text.Json.Serialization;

namespace Bitpin.Responses;

public class GetCurrencyListResponse
{
    [JsonPropertyName("currency")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("tradable")]
    public bool Tradebale { get; set; }

    [JsonPropertyName("precision")]
    public string Precision { get; set; } = null!;
}