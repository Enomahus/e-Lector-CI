using Application;
using Application.Features;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SQLServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools;

namespace ServicesConfiguration;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureAllServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediator();
        services.AddApplicationServices();
        services.AddApplicationFeaturesServices();
        services.AddInfrastructureServices(configuration);
        services.AddInfrastructurePersistenceServices(configuration);
        services.AddInfrastructureSQLServerServices(configuration);
        services.AddInfrastructureIdentityServices(configuration);
        services.AddToolsServices(configuration);

        return services;
    }
}
