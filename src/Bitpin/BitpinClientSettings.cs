namespace Bitpin;

public record BitpinClientSettings
{
    public string Key { get; init; } = null!;

    public string Secret { get; init; } = null!;

    public Uri BaseUrl { get; init; } = null!;

    BitpinClientSettings(string key, string secret, Uri baseUrl)
    {
        Key = key;
        Secret = secret;
        BaseUrl = baseUrl;
    }

    public static BitpinClientSettings Create(string? key, string? secret, string? baseUrl)
    {
        if(String.IsNullOrEmpty(key))
            throw new ArgumentNullException($"{nameof(key)} cannot be null");
        
        if(String.IsNullOrEmpty(secret))
            throw new ArgumentNullException($"{nameof(secret)} cannot be null");
        
        if(String.IsNullOrEmpty(baseUrl))
            throw new ArgumentNullException($"{nameof(baseUrl)} cannot be null");
        
        if(Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            throw new ArgumentException($"{nameof(baseUrl)} must be a valid absolute url");

        return new BitpinClientSettings(key, secret, baseUri!);
    }
}