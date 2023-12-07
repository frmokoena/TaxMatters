using Microsoft.Extensions.DependencyInjection;

namespace Tax.Matters.Web.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebCoreServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

            return services;
        }
    }
}
