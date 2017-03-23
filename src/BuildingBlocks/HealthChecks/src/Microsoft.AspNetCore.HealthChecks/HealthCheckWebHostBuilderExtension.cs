// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class HealthCheckWebHostBuilderExtension
    {
        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, int port)
        {
            Guard.ArgumentValid(port > 0 && port < 65536, nameof(port), "Port must be a value between 1 and 65535");

            builder.ConfigureServices(services =>
            {
                var existingUrl = builder.GetSetting(WebHostDefaults.ServerUrlsKey);
                builder.UseSetting(WebHostDefaults.ServerUrlsKey, $"{existingUrl};http://localhost:{port}");

                services.AddSingleton<IStartupFilter>(new HealthCheckStartupFilter(port));
            });
            return builder;
        }

        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, string path)
        {
            Guard.ArgumentNotNull(nameof(path), path);
            // REVIEW: Is there a better URL path validator somewhere?
            Guard.ArgumentValid(!path.Contains("?"), nameof(path), "Path cannot contain query string values");
            Guard.ArgumentValid(path.StartsWith("/"), nameof(path), "Path should start with /");

            builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new HealthCheckStartupFilter(path)));
            return builder;
        }
    }
}
