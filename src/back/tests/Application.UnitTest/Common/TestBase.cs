using Application.Common.Enums;
using Application.Exceptions;
using Application.Features;
using Application.Models.Errors;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Infrastructure.Persistence.SQLServer.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Tools.Configuration;

namespace Application.UnitTest.Common
{
    public class TestBase
    {
        protected TestBase()
        {
            // Initialisation commune à tous les tests, si nécessaire
        }

        public static IServiceCollection CreateServiceCollection(bool mockAuthorization = true)
        {
            var configuration = new ConfigurationBuilder().Build();

            var services = new ServiceCollection();
            services
                .AddApplicationServices()
                .AddMediator()
                .AddDatabase()
                //.AddInfrastructureIdentityServices(configuration)
                //.AddApolloCoreNetAuthorization()
                //.AddApolloCoreNetAuthorizationPersistence()
                //.AddSingleton(currentUserServiceSub)
                //.AddSingleton(tokenRoleClaimBuilderSub)
                //.AddSingleton(currentUserPermissionsProviderSub)
                //.AddSingleton(currentUserEntityPermissionsProviderSub)
                //.AddSingleton(dateServiceSub)
                //.AddSingleton(blobServiceSub)
                //.AddSingleton(emailServiceSub)
                //.AddSingleton(templatingServiceSub)
                //.AddSingleton(trackDechetGatewaySub)
                //.AddSingleton(siretServiceSub)
                //.AddSingleton(cityServiceSub)
                //.AddSingleton(stringLocalizerSub)
                //.AddScoped<ITokenService, TokenService>()
                //.AddScoped<ITokenHelper, TokenHelper>()
                //.AddScoped<IFileService, FileService>()
                //.AddScoped<IAddressService, AddressService>()
                //.AddScoped<IDataIntegrationService, DataIntegrationService>()
                //.AddScoped<ILogisticSchemeService, LogisticSchemeService>()
                //.AddScoped<RemovalSchemeOptionsService>()
                //.AddScoped<ShippingSchemeOptionsService>()
                //.AddScoped<LogisticRequestService>()
                //.AddScoped<ManualLogisticSchemeService>()
                //.AddScoped<WasteTrackingFormParentStatusesService>()
                //.AddScoped<SetRemovalLogisticRequestAlertesService>()
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

            var serviceProvider = services.BuildServiceProvider();
            var seeder = ActivatorUtilities.CreateInstance<DataSeeder>(serviceProvider);
            seeder.SeedDataAsync().Wait();

            if (mockAuthorization)
            {
                var substitute = Substitute.For<IAuthorizationHandler>();
                //substitute
                //    .HandleAsync()
                //    .Returns(Task.FromResult(new AuthorizationResult() { IsAuthorized = true }));
                var descriptor = new ServiceDescriptor(
                    typeof(IAuthorizationHandler),
                    p => substitute,
                    ServiceLifetime.Transient
                );
                services.Replace(descriptor);
                //currentUserPermissionsProviderSub
                //    .IsCurrentUserAuthenticatedAsync()
                //    .Returns(Task.FromResult(true));
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
