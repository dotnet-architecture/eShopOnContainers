// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class HealthCheckWebHostBuilderExtension
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, int port)
            => UseHealthChecks(builder, port, DefaultTimeout);

        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, int port, TimeSpan timeout)
        {
            Guard.ArgumentValid(port > 0 && port < 65536, nameof(port), "Port must be a value between 1 and 65535.");
            Guard.ArgumentValid(timeout > TimeSpan.Zero, nameof(timeout), "Health check timeout must be a positive time span.");

            builder.ConfigureServices(services =>
            {
                var existingUrl = builder.GetSetting(WebHostDefaults.ServerUrlsKey);
                builder.UseSetting(WebHostDefaults.ServerUrlsKey, $"{existingUrl};http://localhost:{port}");

                services.AddSingleton<IStartupFilter>(new HealthCheckStartupFilter(port, timeout));
            });
            return builder;
        }

        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, string path)
            => UseHealthChecks(builder, path, DefaultTimeout);

        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, string path, TimeSpan timeout)
        {
            Guard.ArgumentNotNull(nameof(path), path);
            // REVIEW: Is there a better URL path validator somewhere?
            Guard.ArgumentValid(!path.Contains("?"), nameof(path), "Path cannot contain query string values.");
            Guard.ArgumentValid(path.StartsWith("/"), nameof(path), "Path should start with '/'.");
            Guard.ArgumentValid(timeout > TimeSpan.Zero, nameof(timeout), "Health check timeout must be a positive time span.");

            builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new HealthCheckStartupFilter(path, timeout)));
            return builder;
        }
    }
}
