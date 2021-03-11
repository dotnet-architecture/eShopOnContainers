using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;

static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(ConnectionMultiplexer connectionMultiplexer)
    {
        if (connectionMultiplexer == null) throw new ArgumentException("!!!!conn is null!");
        var exportType = Environment.GetEnvironmentVariable("OTEL_USE_EXPORTER")?.ToLower();
        if (exportType == null)
        {
            return;
        }

        var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder();

        // Configure resource
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Basket.API"));

        // Configure instrumentation
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation(connectionMultiplexer);

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

        tracerProviderBuilder.Build();
    }
}
