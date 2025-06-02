

using Bitpin.Responses;

namespace Bitpin;

public class BitpinClientService(BitpinRestClient client)
{
    public async Task<IEnumerable<GetMarketListResponse>?> GetMarketsListAsync()
        => await client.GetAsync<IEnumerable<GetMarketListResponse>>("mkt/markets/");


}