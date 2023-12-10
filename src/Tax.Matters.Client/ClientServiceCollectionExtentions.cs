using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tax.Matters.Client;

/// <summary>
/// Injects client services
/// </summary>
public static class ClientServiceCollectionExtentions
{
    public static IServiceCollection AddAPIClient(this IServiceCollection services)
    {
        services.AddSingleton<IHttpClientFactory, HttpClientFactory>();

        services.AddSingleton<IAPIClient, APIClient>();

        // Performance Note:
        //     The http client needs to be singleton to prevent the app from running out of connections.
        services.TryAddSingleton(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return factory.CreateHttpClient();
        });

        return services;
    }
}
