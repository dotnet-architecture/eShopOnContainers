// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.HealthChecks;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.HealthChecks
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;
        private readonly int? _port;
        private readonly IHealthCheckService _service;
        private readonly TimeSpan _timeout;

        public HealthCheckMiddleware(RequestDelegate next, IHealthCheckService service, int port, TimeSpan timeout)
        {
            _port = port;
            _service = service;
            _next = next;
            _timeout = timeout;
        }

        public HealthCheckMiddleware(RequestDelegate next, IHealthCheckService service, string path, TimeSpan timeout)
        {
            _path = path;
            _service = service;
            _next = next;
            _timeout = timeout;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsHealthCheckRequest(context))
            {
                var timeoutTokenSource = new CancellationTokenSource(_timeout);
                var result = await _service.CheckHealthAsync(timeoutTokenSource.Token);
                var status = result.CheckStatus;

                if (status != CheckStatus.Healthy)
                    context.Response.StatusCode = 503;

                context.Response.Headers.Add("content-type", "application/json");
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { status = status.ToString() }));
                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private bool IsHealthCheckRequest(HttpContext context)
        {
            if (_port.HasValue)
            {
                var connInfo = context.Features.Get<IHttpConnectionFeature>();
                if (connInfo.LocalPort == _port)
                    return true;
            }

            if (context.Request.Path == _path)
            {
                return true;
            }

            return false;
        }
    }
}