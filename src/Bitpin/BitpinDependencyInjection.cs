using Microsoft.Extensions.DependencyInjection;

namespace Bitpin;

public static class BitpinDependencyInjection
{
    public static IServiceCollection AddBitpinClient(this IServiceCollection services, string? key, string?  secret, string? baseUrl)
    {
        services.AddSingleton<BitpinRestClient>();
        services.AddSingleton(BitpinClientSettings.Create(key, secret, baseUrl));
        services.AddScoped<BitpinClientService>();

        return services;
    }
}