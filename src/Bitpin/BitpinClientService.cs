

using Bitpin.Responses;

namespace Bitpin;

public class BitpinClientService(BitpinRestClient client)
{
    public async Task<IEnumerable<GetMarketListResponse>?> GetMarketsListAsync()
        => await client.GetAsync<IEnumerable<GetMarketListResponse>>("mkt/markets/");

    public async Task<IEnumerable<GetTickerListResponse?>> GetTickersListAsync()
        => await client.GetAsync<IEnumerable<GetTickerListResponse>>("mkt/tickers/");
    
    public async Task<IEnumerable<GetWalletListResponse>?> GetWalletsListAsync()
        => await client.GetAsync<IEnumerable<GetWalletListResponse>>("wlt/wallets/");
    
    public async Task<IEnumerable<GetMatchListResponse>?> GetMatchesListAsync(string symbol)
        => await client.GetAsync<IEnumerable<GetMatchListResponse>>($"mth/matches/{symbol}/");
    
    public async Task<GetOrderbookResponse?> GetOrderbookAsync(string symbol)
        => await client.GetAsync<GetOrderbookResponse>($"mth/orderbook/{symbol}/");
    
    public async Task<IEnumerable<GetCurrencyListResponse?>> GetCurrenciesListAsync()
        => await client.GetAsync<IEnumerable<GetCurrencyListResponse>>("mkt/currencies/");
    
    
}