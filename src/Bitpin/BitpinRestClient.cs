using System.Net.Http.Json;
using Bitpin.Requests;
using Bitpin.Responses;

namespace Bitpin;

public class BitpinRestClient
{ 
    private readonly BitpinClientSettings _settings;
    private readonly HttpClient _httpClient = new();
    private string? _accessToken = String.Empty;
    private string? _refreshToken = String.Empty;
    private DateTime _expirationTime;
    private const int TokenExpiryInMinutes = 13;
    
    public BitpinRestClient(BitpinClientSettings settings)
    {
        _settings = settings;
        
        // Set the base address for the API
        _httpClient.BaseAddress = _settings.BaseUrl;

        //set default headers
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "UTF-8");
    }
    
    private async Task<GetTokenResponse?> GetTokenAsync()
    {
        var result = await PostAsync<GetTokenRequest,GetTokenResponse>("usr/authenticate/", new GetTokenRequest()
        {
            Key = _settings.Key,
            Secret = _settings.Secret,
        });

        return result;
    }

    private async Task<RefreshTokenResponse?> RefreshTokenAsync()
    {
        if(String.IsNullOrEmpty(_refreshToken))
            throw new InvalidOperationException("No refresh token provided");
        
        var result =await PostAsync<RefreshTokenRequest,RefreshTokenResponse>("usr/refresh_token/", new RefreshTokenRequest()
        {
            RefreshToken = _refreshToken,
        });
        
        return result;
    }

    public async Task<string> TokenHandler()
    {
        if (String.IsNullOrEmpty(_accessToken))
        {
            var tokens=await GetTokenAsync();
            _accessToken = tokens.AccessToken;
            _refreshToken = tokens.RefreshToken;
        }

        if (!String.IsNullOrEmpty(_accessToken) &&
            !String.IsNullOrEmpty(_refreshToken) &&
            _expirationTime.AddMinutes(TokenExpiryInMinutes) < DateTime.UtcNow)
        {
            var tokens=await RefreshTokenAsync();
            _accessToken = tokens.AccessToken;
        }

        _expirationTime= DateTime.UtcNow.AddMinutes(TokenExpiryInMinutes);
        
        return _accessToken;
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint with the given request payload and returns the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request payload.</typeparam>
    /// <typeparam name="TResponse">The type of the response payload.</typeparam>
    /// <param name="endpoint">The relative endpoint URL.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deserialized response object.</returns>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint)) 
            throw new ArgumentException("Endpoint must not be null or empty.", nameof(endpoint));
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(request)
            };

            var token = await TokenHandler();
            
            httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
            
            var response = await _httpClient.PostAsJsonAsync(endpoint, httpRequest, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Sends a GET request to the specified endpoint and returns the response.
    /// </summary>
    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint)) 
            throw new ArgumentException("Endpoint must not be null or empty.", nameof(endpoint));

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
            
            var token = await TokenHandler();
            
            httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
            
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
        }
    }
}