using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Tax.Matters.API.Core.Security;

namespace Tax.Matters.API.Core;

/// <summary>
/// Inject core api services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Inject default api authentication into the pipeline
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBasicHeaderAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = AuthenticationConstants.APIKeyAuthenticationScheme;
        }).AddBasicHeaderAuthentication(AuthenticationConstants.APIKeyAuthenticationScheme, "api key basic authentication scheme", options =>
        {
            options.AuthenticationType = "Basic";
        });

        services.AddSingleton<
            IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandlerCore>();

        return services;
    }

    /// <summary>
    /// Register core handlers 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAPICoreServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }
}
