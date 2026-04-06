using Application.Resources.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Application.Resources
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddResourcesServices(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            return services;
        }
    }
}
