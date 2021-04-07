using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace Catalog.API.Extensions
{
    static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services)
        {
            var exportType = Environment.GetEnvironmentVariable("OTEL_USE_EXPORTER")?.ToLower();
            if (exportType == null)
            {
                return services;
            }

            return services.AddOpenTelemetryTracing((serviceProvider, tracerProviderBuilder) =>
            {
                // Configure resource
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Catalog.API"));

                // Configure instrumentation
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddSqlClientInstrumentation();

                // Configure exporter
                switch (exportType)
                {
                    case "jaeger":
                        tracerProviderBuilder.AddJaegerExporter(options =>
                        {
                            var agentHost = Environment.GetEnvironmentVariable("OTEL_EXPORTER_JAEGER_AGENTHOST");
                            options.AgentHost = agentHost;
                        });
                        break;
                    case "otlp":
                        tracerProviderBuilder.AddOtlpExporter(options =>
                        {
                            var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT")
                                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                            options.Endpoint = new Uri(endpoint);

                            var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_HEADERS")
                                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
                            options.Headers = headers;
                        });
                        break;
                    case "zipkin":
                        tracerProviderBuilder.AddZipkinExporter(options =>
                        {
                            var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_ZIPKIN_ENDPOINT");
                            options.Endpoint = new Uri(endpoint);
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
