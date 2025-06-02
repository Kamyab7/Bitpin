using System.Text.Json.Serialization;

namespace Bitpin.Responses;

public class GetOrderbookResponse
{
    [JsonPropertyName("asks")]
    public List<List<string>> Asks { get; set; } = new();

    [JsonPropertyName("bids")]
    public List<List<string>> Bids { get; set; } = new();
}