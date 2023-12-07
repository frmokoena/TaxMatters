using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainDbContext(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
        {
            services.AddDbContext<AppDbContext>(
                options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(connectionStringName),
                        builder => builder.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext))?.FullName)));
            return services;
        }
    }
}
