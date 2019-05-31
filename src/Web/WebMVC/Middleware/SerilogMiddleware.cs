using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// taken from https://blog.datalust.co/smart-logging-middleware-for-asp-net-core/

namespace Datalust.SerilogMiddlewareExample.Diagnostics
{
    public class SerilogMiddleware
    {
        private const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly ILogger Logger = Serilog.Log.ForContext<SerilogMiddleware>();

        private readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var sw = Stopwatch.StartNew();
            try
            {
                var request = httpContext.Request;

                request.EnableBuffering();

                using (var reader = new StreamReader(request.Body))
                {
                    var headers = string.Join("\n", request.Headers.Select(h => $"{h.Key}: {h.Value}").ToList());
                    var body = await reader.ReadToEndAsync();

                    Logger
                        .ForContext("RequestHeaders", headers)
                        .ForContext("RequestBody", body)
                        .ForContext("RequestQuery", request.QueryString)
                        .Verbose("Request details - {Protocol} {RequestMethod} {Scheme}://{Host}{RequestPath}",
                            request.Protocol, request.Method, request.Scheme, request.Host, request.Path);

                    httpContext.Request.Body.Position = 0;

                    await _next(httpContext);
                }

                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Verbose;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Logger;
                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, sw.Elapsed.TotalMilliseconds);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, sw, ex)) { }
        }

        static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();

            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Logger
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }
    }
}