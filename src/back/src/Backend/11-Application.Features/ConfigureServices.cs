using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureServices
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(executingAssembly));
            services.AddValidatorsFromAssembly(executingAssembly);

            return services;
        }

        public static IServiceCollection AddApplicationFeaturesServices(
            this IServiceCollection services
        )
        {
            //services.AddScoped<LogisticRequestService>();
            //services.AddScoped<WasteTrackingFormParentStatusesService>();
            //services.AddScoped<RemovalSchemeOptionsService>();
            //services.AddScoped<ShippingSchemeOptionsService>();
            //services.AddScoped<ManualLogisticSchemeService>();
            //services.AddScoped<IAddressService, AddressService>();
            //services.AddScoped<ILogisticSchemeService, LogisticSchemeService>();
            //services.AddScoped<SetRemovalLogisticRequestAlertesService>();
            //services.AddScoped<ITokenHelper, TokenHelper>();

            return services;
        }
    }
}
