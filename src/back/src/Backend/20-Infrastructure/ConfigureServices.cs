using Application.Interfaces.Services;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IDataIntegrationService, DataIntegrationService>();

        services.Configure<TokenConfiguration>(configuration.GetSection("JwtConfig"));
        //services.Configure<PdfPrinterConfiguration>(configuration.GetSection("Service:Print"));

        return services;
    }
}
