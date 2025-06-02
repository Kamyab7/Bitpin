using System.Net.Http.Json;
using Bitpin.Requests;
using Bitpin.Responses;

namespace Bitpin;

public class BitpinRestClient
{
    private readonly BitpinClientSettings _settings;
    private readonly HttpClient _httpClient = new();
    private string? _accessToken = string.Empty;
    private string? _refreshToken = string.Empty;
    private DateTime _accessTokenExpirationTime;
    private DateTime _refreshTokenExpirationTime;

    private const int TokenExpiryInMinutes = 13;
    private const int RefreshTokenExpiryInDays = 29;

    public BitpinRestClient(BitpinClientSettings settings)
    {
        _settings = settings;
        _httpClient.BaseAddress = _settings.BaseUrl;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "UTF-8");
    }

    private async Task<GetTokenResponse?> GetTokenAsync()
    {
        return await SendRequestAsync<GetTokenRequest, GetTokenResponse>("usr/authenticate/", new GetTokenRequest
        {
            Key = _settings.Key,
            Secret = _settings.Secret
        });
    }

    private async Task<RefreshTokenResponse?> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken))
            throw new InvalidOperationException("No refresh token provided.");

        return await SendRequestAsync<RefreshTokenRequest, RefreshTokenResponse>("usr/refresh_token/", new RefreshTokenRequest
        {
            RefreshToken = _refreshToken
        });
    }

    private async Task RetrieveAndStoreNewTokenAsync()
    {
        var tokens = await GetTokenAsync();
        if (tokens is null)
            throw new InvalidOperationException("Failed to retrieve tokens.");

        _accessToken = tokens.AccessToken;
        _refreshToken = tokens.RefreshToken;
        _refreshTokenExpirationTime = DateTime.UtcNow.AddDays(RefreshTokenExpiryInDays);
    }

    private async Task<string> TokenHandler()
    {
        var now = DateTime.UtcNow;
        bool isAccessTokenMissing = string.IsNullOrEmpty(_accessToken);
        bool isAccessTokenExpired = _accessTokenExpirationTime.AddMinutes(TokenExpiryInMinutes) < now;
        bool isRefreshTokenExpired = _refreshTokenExpirationTime.AddDays(RefreshTokenExpiryInDays) < now;

        if (isAccessTokenMissing)
        {
            await RetrieveAndStoreNewTokenAsync();
        }
        else if (!isRefreshTokenExpired && isAccessTokenExpired)
        {
            var tokens = await RefreshTokenAsync();
            _accessToken = tokens.AccessToken;
        }
        else if (!isRefreshTokenExpired)
        {
            await RetrieveAndStoreNewTokenAsync();
        }

        _accessTokenExpirationTime = now.AddMinutes(TokenExpiryInMinutes);
        return _accessToken!;
    }

    private async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        var token = await TokenHandler();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        return await HandleResponse<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint must not be null or empty.", nameof(endpoint));

        var token = await TokenHandler();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        return await HandleResponse<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint must not be null or empty.", nameof(endpoint));

        var token = await TokenHandler();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        return await HandleResponse<TResponse>(response, cancellationToken);
    }
    
    /// <summary>
    /// Sends a DELETE request to the specified endpoint and returns the response.
    /// </summary>
    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint must not be null or empty.", nameof(endpoint));

        var token = await TokenHandler();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");

        await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<TResponse?> HandleResponse<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
    }
}
