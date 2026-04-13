using Application.Features.RegistrationRequests.Common;
using Application.Features.Security.Common;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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
            services.AddScoped<RegistrationRequestService>();
            //services.AddScoped<WasteTrackingFormParentStatusesService>();
            //services.AddScoped<RemovalSchemeOptionsService>();
            //services.AddScoped<ShippingSchemeOptionsService>();
            //services.AddScoped<ManualLogisticSchemeService>();
            //services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IReferenceGeneratorService,ReferenceGeneratorService>();
            //services.AddScoped<SetRemovalLogisticRequestAlertesService>();
            services.AddScoped<ITokenHelper, TokenHelper>();

            return services;
        }
    }
}
