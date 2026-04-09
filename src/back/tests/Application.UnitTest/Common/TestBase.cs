using Application.Common.Enums;
using Application.Exceptions;
using Application.Features;
using Application.Features.RegistrationRequests.Common;
using Application.Features.Security.Common;
using Application.Interfaces.Services;
using Application.Models.Errors;
using Application.Resources;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.ExternalAuth;
using Infrastructure.ExternalAuth.Interfaces;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.File.Services;
using Infrastructure.Persistence.SQLServer;
using Infrastructure.Persistence.SQLServer.Contexts;
using Infrastructure.Persistence.SQLServer.Seeders;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Pcea.Core.Net.Authorization;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Pcea.Core.Net.Authorization.Interfaces.Handlers;
using Pcea.Core.Net.Authorization.Models;
using Pcea.Core.Net.Authorization.Persistence;
using Pcea.Core.Net.Authorization.Web.Interfaces.Services;
using System.Globalization;
using Tools.Configuration;
using Web.Services;

namespace Application.UnitTest.Common
{
    public class TestBase
    {
        protected TestBase()
        {
            // Initialisation commune à tous les tests, si nécessaire
        }

        public static IServiceCollection CreateServiceCollection(
            Action<TimeProvider>? setupDateService = null,
            Action<IStringLocalizer<ApplicationResources>>? stringLocalizer = null,
            bool mockAuthorization = true)
        {
            var currentUserServiceSub = Substitute.For<ICurrentUserService>();
            var tokenRoleClaimBuilderSub = Substitute.For<ITokenRoleClaimBuilder<long>>();
            var currentUserPermissionsProviderSub = Substitute.For<ICurrentUserPermissionsProvider>();
            var currentUserEntityPermissionsProviderSub = Substitute.For<
                ICurrentUserEntityPermissionsProvider<long>
            >();

            var timeProviderSub = Substitute.For<TimeProvider>();
            timeProviderSub.GetUtcNow().Returns(new DateTimeOffset(2026,1,1,10,0,0,TimeSpan.Zero));

            var stringLocalizerSub = Substitute.For<IStringLocalizer<ApplicationResources>>();
            var externalAuthSub = Substitute.For<IExternalAuthService>();

            setupDateService?.Invoke(timeProviderSub);
            stringLocalizer?.Invoke(stringLocalizerSub);

            var configuration = new ConfigurationBuilder().Build();

            var services = new ServiceCollection();
            services
                .AddApplicationServices()
                .AddMediator()
                .AddDatabase(timeProviderSub)
                .AddInfrastructureIdentityServices(configuration)
                .AddPceaCoreNetAuthorization()
                .AddPceaCoreNetAuthorizationPersistence()
                .AddSingleton(currentUserServiceSub)
                .AddSingleton(tokenRoleClaimBuilderSub)
                .AddSingleton(currentUserPermissionsProviderSub)
                .AddSingleton(currentUserEntityPermissionsProviderSub)
                .AddSingleton(timeProviderSub)
                //.AddSingleton(blobServiceSub)
                //.AddSingleton(emailServiceSub)
                //.AddSingleton(templatingServiceSub)
                //.AddSingleton(trackDechetGatewaySub)
                //.AddSingleton(siretServiceSub)
                //.AddSingleton(cityServiceSub)
                .AddSingleton(stringLocalizerSub)
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<ITokenHelper, TokenHelper>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IReferenceGeneratorService, ReferenceGeneratorService>()
                .AddScoped<IDataIntegrationService, DataIntegrationService>()
                //.AddScoped<ILogisticSchemeService, LogisticSchemeService>()
                //.AddScoped<RemovalSchemeOptionsService>()
                //.AddScoped<ShippingSchemeOptionsService>()
                //.AddScoped<LogisticRequestService>()
                //.AddScoped<ManualLogisticSchemeService>()
                //.AddScoped<WasteTrackingFormParentStatusesService>()
                //.AddScoped<SetRemovalLogisticRequestAlertesService>()
                .AddKeyedSingleton(ExternalAuthServiceKeys.GoogleAuthService, externalAuthSub)
                .AddKeyedSingleton(ExternalAuthServiceKeys.MicrosoftAuthService, externalAuthSub)
                .Configure<TokenConfiguration>(c =>
                    c.Secret = "DEV_SECRET_JWT_KEY_VERY_LONG_FOR_SECURITY_PURPOSES"
                )
                .Configure<AppConfiguration>(c =>
                {
                    c.AppUrl = "http://localhost:44082";
                })
                .Configure<DataConfiguration>(c =>
                {
                    c.Seed = true;
                    c.DefaultUserPassword = "Secret01";
                });

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentUICulture = CultureInfo.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            var serviceProvider = services.BuildServiceProvider();
            var seeder = ActivatorUtilities.CreateInstance<DataSeeder>(serviceProvider);
            seeder.SeedDataAsync().Wait();

            if (mockAuthorization)
            {
                var substitute = Substitute.For<IAuthorizationHandler>();
                substitute
                    .HandleAsync()
                    .Returns(Task.FromResult(new AuthorizationResult() { IsAuthorized = true }));
                var descriptor = new ServiceDescriptor(
                    typeof(IAuthorizationHandler),
                    p => substitute,
                    ServiceLifetime.Transient
                );
                services.Replace(descriptor);
                currentUserPermissionsProviderSub
                    .IsCurrentUserAuthenticatedAsync()
                    .Returns(Task.FromResult(true));
            }

            return services;
        }

        protected static async Task<ConstituencyDao> CreateConstituencyAsync(
            WritableDbContext context,
            string name,
            LocationLevel level,
            long? parentId
        )
        {
            var constituency = new ConstituencyDao()
            {
                Wording = name ?? "Name",
                Level = level,
                ParentId = level == LocationLevel.Region ? null : parentId,
            };

            await context.Constituencies.AddAsync(constituency);
            await context.SaveChangesAsync();

            return constituency;
        }

        #region Assert helpers
        protected static void AssertValidationException(
            ValidationException exception,
            string key,
            ValidationErrorCode code
        )
        {
            exception.AdditionalData.Should().ContainSingle();
            var error = exception.AdditionalData.Single();
            error.Key.Should().Be(key);
            error.Value.Should().Be(code.ToString());
        }

        protected static void AssertValidationException(
            ValidationException exception,
            string key,
            string message
        )
        {
            exception.AdditionalData.Should().ContainSingle();
            var error = exception.AdditionalData.Single();
            error.Key.Should().Be(key);
            error.Value.Should().Be(message);
        }

        protected static void AssertValidationException(
            IEnumerable<ValidationException> exceptions,
            string key,
            ValidationErrorCode code
        )
        {
            exceptions.Should().ContainSingle();
            var exception = exceptions.Single();
            AssertValidationException(exception, key, code);
        }

        protected static void AssertValidationException(
            IEnumerable<ValidationException> exceptions,
            params KeyValuePair<string, ValidationErrorCode>[] values
        )
        {
            exceptions.Should().ContainSingle();
            var exception = exceptions.Single();

            var valuesGroupBy = values
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(v => v.Value).ToArray());

            foreach (var value in valuesGroupBy)
            {
                AssertValidationException(exception, value.Key, value.Value);
            }
        }

        protected static void AssertValidationException(
            IEnumerable<ValidationException> exceptions,
            params KeyValuePair<string, string>[] values
        )
        {
            exceptions.Should().ContainSingle();
            var exception = exceptions.Single();

            var valuesGroupBy = values
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(v => v.Value).ToArray());

            foreach (var value in valuesGroupBy)
            {
                AssertValidationException(exception, value.Key, value.Value);
            }
        }

        protected static void AssertValidationException(
            ValidationException exception,
            params KeyValuePair<string, ValidationErrorCode>[] values
        )
        {
            var valuesGroupBy = values
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(v => v.Value).ToArray());

            foreach (var value in valuesGroupBy)
            {
                AssertValidationException(exception, value.Key, value.Value);
            }
        }

        protected static void AssertValidationException(
            ValidationException exception,
            params KeyValuePair<string, string>[] values
        )
        {
            var valuesGroupBy = values
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(v => v.Value).ToArray());

            foreach (var value in valuesGroupBy)
            {
                AssertValidationException(exception, value.Key, value.Value);
            }
        }

        protected static void AssertValidationException(
            ValidationException exception,
            string key,
            params ValidationErrorCode[] codes
        )
        {
            var value = string.Join(" ", codes);
            exception
                .AdditionalData.Should()
                .ContainEquivalentOf(new KeyValuePair<string, string>(key, value));
        }

        protected static void AssertValidationException(
            ValidationException exception,
            string key,
            params string[] messages
        )
        {
            var rule = exception.AdditionalData.Any(v =>
                v.Key == key && v.Value == string.Join(" ", messages)
            );
            rule.Should().BeTrue();
        }
        #endregion
    }
}
