using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.Configuration;

namespace Tools;

public static class ConfigureServices
{
    public static IServiceCollection AddToolsServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var configSection = configuration.GetSection("AppConfig");
        //if (string.IsNullOrEmpty(configSection.GetValue<string>("SorenSiret")))
        //{
        //    throw new ConfigurationMissingException("Missing configuration AppConfig.SorenSiret");
        //}
        services.Configure<AppConfiguration>(configSection);

        return services;
    }
}
