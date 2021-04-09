using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.API.Extensions
{
    static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services)
        {
            var exportType = Environment.GetEnvironmentVariable("OTEL_USE_EXPORTER")?.ToLower();
            var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_TOOL_ENDPOINT")?.ToLower();

            if (exportType == null)
            {
                return services;
            }

            return services.AddOpenTelemetryTracing((serviceProvider, tracerProviderBuilder) =>
            {
                // Configure resource
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Payment.API"));

                // Configure instrumentation
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                // Configure exporter
                switch (exportType)
                {
                    case "jaeger":
                        tracerProviderBuilder.AddJaegerExporter(options =>
                        {                            
                            options.AgentHost = endpoint;
                        });
                        break;
                    case "otlp":
                        tracerProviderBuilder.AddOtlpExporter(options =>
                        {                            
                            options.Endpoint = new Uri(endpoint);

                            var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_HEADERS")
                                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
                            options.Headers = headers;
                        });
                        break;
                    case "zipkin":
                        tracerProviderBuilder.AddZipkinExporter(options =>
                        {                            
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
