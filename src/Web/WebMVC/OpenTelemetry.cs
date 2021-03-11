using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

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
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebMVC"));

            // Configure instrumentation
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            // Configure exporter
            switch (exportType)
            {
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
                default:
                    tracerProviderBuilder.AddConsoleExporter();
                    break;
            }
        });
    }
}
