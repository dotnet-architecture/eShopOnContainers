// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.HealthChecks
{
    public class HealthCheckStartupFilter : IStartupFilter
    {
        private string _path;
        private int? _port;

        public HealthCheckStartupFilter(int port)
        {
            _port = port;
        }

        public HealthCheckStartupFilter(string path)
        {
            _path = path;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                if (_port.HasValue)
                    app.UseMiddleware<HealthCheckMiddleware>(_port);
                else
                    app.UseMiddleware<HealthCheckMiddleware>(_path);

                next(app);
            };
        }
    }
}