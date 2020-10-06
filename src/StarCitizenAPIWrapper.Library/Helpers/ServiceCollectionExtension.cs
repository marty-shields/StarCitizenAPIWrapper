using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StarCitizenAPIWrapper.Library.Helpers
{
    /// <summary>
    /// Extension for the <see cref="IServiceCollection"/> to use the <see cref="StarCitizenClient"/>.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds the <see cref="StarCitizenClient"/> to the service collection.
        /// </summary>
        public static IServiceCollection AddStarCitizenClient(this IServiceCollection services)
        {
            services.AddScoped(s =>
            {
                var client = s.GetRequiredService<IHttpClientFactory>().CreateClient();
                var config = s.GetRequiredService<IConfiguration>();
                return new StarCitizenClient(config, client);
            });
            return services;
        }
    }
}
