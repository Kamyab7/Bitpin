

using Bitpin.Requests;
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
    
    public async Task<IEnumerable<GetCompeletedOrderListResponse?>> GetCompletedOrdersAsync()
        => await client.GetAsync<IEnumerable<GetCompeletedOrderListResponse>>("odr/fills/");
    
    public async Task<IEnumerable<GetPendingOrderListResponse>?> GetPendingOrdersAsync()
        => await client.GetAsync<IEnumerable<GetPendingOrderListResponse>>("odr/orders/");
    
    public async Task<GetOrderByIdResponse?> GetOrderByIdAsync(int orderId)
        => await client.GetAsync<GetOrderByIdResponse>($"odr/orders/{orderId}/");
    
    public async Task<IEnumerable<GetOrderLisrResponse?>> GetOrderListAsync()
        => await client.GetAsync<IEnumerable<GetOrderLisrResponse>>($"odr/orders/");
    
    public async Task CancelOrderAsync(int orderId)
        => await client.DeleteAsync($"odr/orders/{orderId}/");
    
    public async Task<CreateOrderResponse?> CreateLimitOrderAsync(CreateLimitOrderRequest createLimitOrderRequest)
        => await client.PostAsync<CreateLimitOrderRequest,CreateOrderResponse>("odr/orders/", createLimitOrderRequest);

    public async Task<CreateOrderResponse?> CreateMarketOrderAsync(CreateMarketOrderRequest createMarketOrderRequest)
        => await client.PostAsync<CreateMarketOrderRequest,CreateOrderResponse>("odr/orders/", createMarketOrderRequest);

    public async Task<CreateOrderResponse?> CreateStopLimitOrderAsync(CreateStopLimitOrderRequest createStopLimitOrderRequest)
        => await client.PostAsync<CreateStopLimitOrderRequest,CreateOrderResponse>("odr/orders/", createStopLimitOrderRequest);

    public async Task<CreateOrderResponse?> CreateOcoOrderAsync(CreateOcoOrderRequest createOcoOrderRequest)
        => await client.PostAsync<CreateOcoOrderRequest,CreateOrderResponse>("odr/orders/", createOcoOrderRequest);
}