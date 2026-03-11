using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.CodeAnalysis;
using Tools.Logging;

namespace Web.Common
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureTelemetryServices
    {
        public static WebApplicationBuilder AddTelemetryServices(this WebApplicationBuilder builder, ILogger log)
        {
            var metricEnable = builder.Configuration.GetValue<bool>("Metric:Enable");
            var endpoint = builder.Configuration.GetValue<string>("Metric:TracerEndpoint");
            var appInsightConnectionString = builder.Configuration.GetValue<string>(
                "APPLICATIONINSIGHTS_CONNECTION_STRING"
            );

            builder.Services.AddApplicationInsightsTelemetry();

            if (metricEnable && !string.IsNullOrEmpty(endpoint))
            {
                log.LogInformation($"[Metric] Enabled");

                var resourceAttributes = new Dictionary<string, object>
            {
                { "service.namespace", "PortailELECTORCI" },
                { "span.kind", "SERVER" },
            };

                builder.Logging.AddOpenTelemetry(logging =>
                {
                    // The rest of your setup code goes here
                    logging.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new System.Uri(endpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                });

                var otBuilder = builder
                    .Services.AddOpenTelemetry()
                    .ConfigureResource(resourceBuilder =>
                        resourceBuilder.AddService("App").AddAttributes(resourceAttributes)
                    )
                    .WithTracing(tracing =>
                        tracing
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddOtlpExporter(opt =>
                            {
                                opt.Endpoint = new System.Uri(endpoint);
                                opt.Protocol = OtlpExportProtocol.Grpc;
                            })
                    )
                    .WithMetrics(builder =>
                        builder
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddMeter("Microsoft.AspNetCore.Hosting")
                            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                            .AddOtlpExporter(opt =>
                            {
                                opt.Endpoint = new System.Uri(endpoint);
                                opt.Protocol = OtlpExportProtocol.Grpc;
                            })
                    );

                if (!string.IsNullOrEmpty(appInsightConnectionString))
                {
                    otBuilder.UseAzureMonitorExporter();
                }

                AddTracerProvider(
                        "REPOSITORY",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.REPOSITORY.Name
                    )
                    .Build();
                AddTracerProvider(
                        "CONTROLLER",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.CONTROLLER.Name
                    )
                    .Build();
                AddTracerProvider(
                        "INFRASTRUCTURE",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.INFRASTRUCTURE.Name
                    )
                    .Build();
                AddTracerProvider(
                        "TOOLS",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.TOOLS.Name
                    )
                    .Build();
                AddTracerProvider(
                        "CQRS",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.CQRS.Name
                    )
                    .Build();
                AddTracerProvider(
                        "WEB",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.WEB.Name
                    )
                    .Build();

                AddTracerProvider(
                        "EF",
                        endpoint,
                        appInsightConnectionString,
                        resourceAttributes,
                        ActivitySourceLog.EF.Name
                    )
                    .AddEntityFrameworkCoreInstrumentation(
                        Infrastructure
                            .Persistence
                            .SQLServer
                            .ConfigureTelemetryServices
                            .AddEntityFrameworkCoreTelemetry
                    )
                    .Build();

                log.LogInformation($"[Metric] start");
            }
            else
            {
                log.LogInformation($"[Metric] Disabled");
            }

            return builder;
        }


        private static TracerProviderBuilder AddTracerProvider(
            string name,
            string endpoint,
            string appInsightConnectionString,
            Dictionary<string, object> resourceAttributes,
            params string[] sources
        )
        {
            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(name)
                .AddAttributes(resourceAttributes);

            var builder = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddSource(sources)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new System.Uri(endpoint);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });

            if (!string.IsNullOrEmpty(appInsightConnectionString))
            {
                builder = builder.AddAzureMonitorTraceExporter(opt =>
                {
                    opt.ConnectionString = appInsightConnectionString;
                });
            }

            return builder;
        }

        private static MeterProviderBuilder AddMeterProvider(
            string name,
            string endpoint,
            string appInsightConnectionString,
            Dictionary<string, object> resourceAttributes,
            params string[] sources
        )
        {
            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(name)
                .AddAttributes(resourceAttributes);

            var builder = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                //.AddMeter(sources)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new System.Uri(endpoint);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });

            if (!string.IsNullOrEmpty(appInsightConnectionString))
            {
                builder = builder.AddAzureMonitorMetricExporter(opt =>
                {
                    opt.ConnectionString = appInsightConnectionString;
                });
            }

            return builder;
        }
    }
}
