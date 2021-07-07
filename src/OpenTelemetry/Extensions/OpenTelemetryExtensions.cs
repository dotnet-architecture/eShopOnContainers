using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;

namespace OpenTelemetry.Customization.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, OpenTelemetryConfig openTelemetryConfig)
        {            
            if (openTelemetryConfig == null || openTelemetryConfig.ExportType == null)
            {
                return services;
            }

            return services.AddOpenTelemetryTracing((serviceProvider, tracerProviderBuilder) =>
            {                   

                // Configure resource
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(openTelemetryConfig.ServiceName));

                // Configure instrumentation
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddSqlClientInstrumentation();

                // Configure exporter
                switch (openTelemetryConfig.ExportType)
                {
                    case "jaeger":
                        tracerProviderBuilder.AddJaegerExporter(options =>
                        {
                            options.AgentHost = openTelemetryConfig.ExportToolEndpoint;
                        });
                        break;
                    case "otlp":
                        tracerProviderBuilder.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetryConfig.ExportToolEndpoint);

                            var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_HEADERS")
                                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
                            options.Headers = headers;
                        });
                        break;
                    case "zipkin":
                        tracerProviderBuilder.AddZipkinExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetryConfig.ExportToolEndpoint);
                        });
                        break;
                    default:
                        tracerProviderBuilder.AddConsoleExporter();
                        break;
                }
            });
        }

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, ConnectionMultiplexer connectionMultiplexer, OpenTelemetryConfig openTelemetryConfig)
        {            

            if (openTelemetryConfig == null || openTelemetryConfig.ExportType == null)
            {
                return services;
            }

            return services.AddOpenTelemetryTracing((serviceProvider, tracerProviderBuilder) =>
            {                   

                // Configure resource
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(openTelemetryConfig.ServiceName));

                // Configure instrumentation
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddRedisInstrumentation(connectionMultiplexer);

                // Configure exporter
                switch (openTelemetryConfig.ExportType)
                {
                    case "jaeger":
                        tracerProviderBuilder.AddJaegerExporter(options =>
                        {
                            options.AgentHost = openTelemetryConfig.ExportToolEndpoint;
                        });
                        break;
                    case "otlp":
                        tracerProviderBuilder.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetryConfig.ExportToolEndpoint);

                            var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_HEADERS")
                                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
                            options.Headers = headers;
                        });
                        break;
                    case "zipkin":
                        tracerProviderBuilder.AddZipkinExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetryConfig.ExportToolEndpoint);
                        });
                        break;
                    default:
                        tracerProviderBuilder.AddConsoleExporter();
                        break;
                }
            });
            
        }
    }
}
