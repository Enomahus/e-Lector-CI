using Application.Interfaces.Services;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Pcea.Core.Net.Authorization;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Pcea.Core.Net.Authorization.Web.Interfaces.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tools.Constants;
using Web.Common.Authorization;
using Web.Common.Converters;
using Web.Common.Filters;
using Web.Common.Handlers;
using Web.Services;

namespace Web.Common
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);


            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
            {
                services.AddSingleton(version);
            }

            services.AddOpenApiDocument(options =>
            {
                options.Title = "Api";
                options.Version = version?.ToString();
                options.Description = "API for ElectorCI backend platform";
                options.AddSecurity(
                    "JWT",
                    [],
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.Http,
                        Description = "Enter your JWT token :",
                        Scheme = "bearer",
                    }
                );

                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            var allowedOrigins = configuration.GetValue<string>("ApiConfig:AllowedOrigins")?.Split(';');
            if (allowedOrigins is not null)
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder
                            .SetIsOriginAllowed((origin) => allowedOrigins.Contains(origin))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("Content-Disposition");
                    });
                });
            }

            services.ConfigureJWT(configuration);

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            List<JsonConverter> converters =
            [
                new DateOnlyJsonConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            ];
            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }
            options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            services.AddSingleton(options);

            services
            .AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            })
            .AddJsonOptions(o =>
            {
                foreach (var converter in converters)
                {
                    o.JsonSerializerOptions.Converters.Add(converter);
                }
                o.JsonSerializerOptions.NumberHandling = options.NumberHandling;
                o.JsonSerializerOptions.DefaultIgnoreCondition = options.DefaultIgnoreCondition;
            });

            services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();
            services.AddScoped<ITokenRoleClaimBuilder<long>, TokenRoleClaimBuilder>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentUserPermissionsProvider, CurrentUserService>();
            services.AddScoped<ICurrentUserEntityPermissionsProvider<long>, CurrentUserService>();
            services.AddPceaCoreNetAuthorization();

            return services;

        }

        public static Task UseWebServicesAsync(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            var swaggerEnabled = app.Configuration.GetValue<bool>("Swagger:Enable");
            if (swaggerEnabled)
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }

            var supportedCultures = new[] { "fr-FR" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            return Task.CompletedTask;
        }

        private static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var jwtConfigParsed = jwtConfig.Get<TokenConfiguration>() ?? new TokenConfiguration();
            var secretKey = jwtConfigParsed.Secret;
            var validIssuer = jwtConfigParsed.ValidIssuer;

            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = validIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    };
                });

            services.AddAuthorizationBuilder();

            //services
            //    .AddAuthorizationBuilder()
            //    .AddPolicy(RequirePolicy.SuperAdmin, policy => policy.RequireRole(AppConstants.SuperAdminRole))
            //    .AddPolicy(
            //        RequirePolicy.Admin,
            //        policy => policy.RequireRole(AppConstants.SuperAdminRole)
            //    )
            //    .AddPolicy(
            //        RequirePolicy.OrganismAgent,
            //        policy => policy.RequireRole(AppConstants.OrganismAgentRole)
            //    )
            //    .AddPolicy(
            //        RequirePolicy.Elector, 
            //        policy => policy.RequireRole(AppConstants.ElectorRole)
            //    );
        }
    }
}
