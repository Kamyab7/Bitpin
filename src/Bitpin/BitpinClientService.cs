using Bitpin.Requests;
using Bitpin.Responses;

namespace Bitpin;

public class BitpinClientService(BitpinRestClient client)
{
    /// <summary>
    /// Retrieves the full list of trading markets supported on Bitpin.
    /// Includes market symbol, name, base currency, and quote currency.
    /// </summary>
    public async Task<IEnumerable<GetMarketListResponse>?> GetMarketsListAsync()
        => await client.GetAsync<IEnumerable<GetMarketListResponse>>("mkt/markets/");

    /// <summary>
    /// Retrieves the current ticker (price) information for all markets on Bitpin.
    /// </summary>
    public async Task<IEnumerable<GetTickerListResponse?>> GetTickersListAsync()
        => await client.GetAsync<IEnumerable<GetTickerListResponse>>("mkt/tickers/");

    /// <summary>
    /// Retrieves your Bitpin wallet balances across various cryptocurrencies.
    /// </summary>
    public async Task<IEnumerable<GetWalletListResponse>?> GetWalletsListAsync()
        => await client.GetAsync<IEnumerable<GetWalletListResponse>>("wlt/wallets/");

    /// <summary>
    /// Retrieves the latest trades executed in the specified market.
    /// </summary>
    /// <param name="symbol">Market symbol (e.g., btc_irt)</param>
    public async Task<IEnumerable<GetMatchListResponse>?> GetMatchesListAsync(string symbol)
        => await client.GetAsync<IEnumerable<GetMatchListResponse>>($"mth/matches/{symbol}/");

    /// <summary>
    /// Retrieves the current order book for a specified market.
    /// Includes buy (bid) and sell (ask) orders.
    /// </summary>
    /// <param name="symbol">Market symbol</param>
    public async Task<GetOrderbookResponse?> GetOrderbookAsync(string symbol)
        => await client.GetAsync<GetOrderbookResponse>($"mth/orderbook/{symbol}/");

    /// <summary>
    /// Retrieves the list of all cryptocurrencies supported by Bitpin.
    /// Includes symbol, name, tradability, and precision details.
    /// </summary>
    public async Task<IEnumerable<GetCurrencyListResponse?>> GetCurrenciesListAsync()
        => await client.GetAsync<IEnumerable<GetCurrencyListResponse>>("mkt/currencies/");

    /// <summary>
    /// Retrieves your completed/executed order history.
    /// Includes price, amount, and fee information.
    /// </summary>
    public async Task<IEnumerable<GetCompeletedOrderListResponse?>> GetCompletedOrdersAsync()
        => await client.GetAsync<IEnumerable<GetCompeletedOrderListResponse>>("odr/fills/");

    /// <summary>
    /// Retrieves your currently active (pending) orders.
    /// </summary>
    public async Task<IEnumerable<GetPendingOrderListResponse>?> GetPendingOrdersAsync()
        => await client.GetAsync<IEnumerable<GetPendingOrderListResponse>>("odr/orders/");

    /// <summary>
    /// Retrieves the details of a specific order by its order ID.
    /// </summary>
    /// <param name="orderId">The ID of the order</param>
    public async Task<GetOrderByIdResponse?> GetOrderByIdAsync(int orderId)
        => await client.GetAsync<GetOrderByIdResponse>($"odr/orders/{orderId}/");

    /// <summary>
    /// Retrieves the list of all your orders.
    /// </summary>
    public async Task<IEnumerable<GetOrderLisrResponse?>> GetOrderListAsync()
        => await client.GetAsync<IEnumerable<GetOrderLisrResponse>>("odr/orders/");

    /// <summary>
    /// Cancels an existing order using its order ID.
    /// </summary>
    /// <param name="orderId">The ID of the order to cancel</param>
    public async Task CancelOrderAsync(int orderId)
        => await client.DeleteAsync($"odr/orders/{orderId}/");

    /// <summary>
    /// Places a limit order to buy or sell cryptocurrency at a specific price.
    /// The order is only executed when the market reaches your set price.
    /// </summary>
    /// <param name="createLimitOrderRequest">Limit order request details</param>
    public async Task<CreateOrderResponse?> CreateLimitOrderAsync(CreateLimitOrderRequest createLimitOrderRequest)
        => await client.PostAsync<CreateLimitOrderRequest, CreateOrderResponse>("odr/orders/", createLimitOrderRequest);

    /// <summary>
    /// Places a market order to instantly buy or sell cryptocurrency at the best available price.
    /// </summary>
    /// <param name="createMarketOrderRequest">Market order request details</param>
    public async Task<CreateOrderResponse?> CreateMarketOrderAsync(CreateMarketOrderRequest createMarketOrderRequest)
        => await client.PostAsync<CreateMarketOrderRequest, CreateOrderResponse>("odr/orders/", createMarketOrderRequest);

    /// <summary>
    /// Places a stop-limit order. When the stop price is reached, a limit order is triggered.
    /// Useful for risk management and strategic entries/exits.
    /// </summary>
    /// <param name="createStopLimitOrderRequest">Stop-limit order request details</param>
    public async Task<CreateOrderResponse?> CreateStopLimitOrderAsync(CreateStopLimitOrderRequest createStopLimitOrderRequest)
        => await client.PostAsync<CreateStopLimitOrderRequest, CreateOrderResponse>("odr/orders/", createStopLimitOrderRequest);

    /// <summary>
    /// Places an OCO (One Cancels the Other) order.
    /// Combines a limit order and stop-limit order—when one executes, the other is automatically canceled.
    /// </summary>
    /// <param name="createOcoOrderRequest">OCO order request details</param>
    public async Task<CreateOrderResponse?> CreateOcoOrderAsync(CreateOcoOrderRequest createOcoOrderRequest)
        => await client.PostAsync<CreateOcoOrderRequest, CreateOrderResponse>("odr/orders/", createOcoOrderRequest);
}
