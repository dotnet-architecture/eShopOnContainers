// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks.Internal
{
    public class UrlChecker
    {
        private readonly Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> _checkFunc;
        private readonly string[] _urls;

        // REVIEW: Cache timeout here?
        public UrlChecker(Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> checkFunc, params string[] urls)
        {
            Guard.ArgumentNotNull(nameof(checkFunc), checkFunc);
            Guard.ArgumentNotNullOrEmpty(nameof(urls), urls);

            _checkFunc = checkFunc;
            _urls = urls;
        }

        public CheckStatus PartiallyHealthyStatus { get; set; } = CheckStatus.Warning;

        public Task<IHealthCheckResult> CheckAsync()
            => _urls.Length == 1 ? CheckSingleAsync() : CheckMultiAsync();

        public async Task<IHealthCheckResult> CheckSingleAsync()
        {
            var httpClient = CreateHttpClient();
            var result = default(IHealthCheckResult);
            await CheckUrlAsync(httpClient, _urls[0], (_, checkResult) => result = checkResult).ConfigureAwait(false);
            return result;
        }

        public async Task<IHealthCheckResult> CheckMultiAsync()
        {
            var composite = new CompositeHealthCheckResult(PartiallyHealthyStatus);
            var httpClient = CreateHttpClient();

            // REVIEW: Should these be done in parallel?
            foreach (var url in _urls)
                await CheckUrlAsync(httpClient, url, (name, checkResult) => composite.Add(name, checkResult)).ConfigureAwait(false);

            return composite;
        }

        private async Task CheckUrlAsync(HttpClient httpClient, string url, Action<string, IHealthCheckResult> adder)
        {
            var name = $"UrlCheck({url})";
            try
            {
                var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                var result = await _checkFunc(response);
                adder(name, result);
            }
            catch (Exception ex)
            {
                adder(name, HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}"));
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = GetHttpClient();
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            return httpClient;
        }

        public static async ValueTask<IHealthCheckResult> DefaultUrlCheck(HttpResponseMessage response)
        {
            // REVIEW: Should this be an explicit 200 check, or just an "is success" check?
            var status = response.StatusCode == HttpStatusCode.OK ? CheckStatus.Healthy : CheckStatus.Unhealthy;
            var data = new Dictionary<string, object>
            {
                { "url", response.RequestMessage.RequestUri.ToString() },
                { "status", (int)response.StatusCode },
                { "reason", response.ReasonPhrase },
                { "body", await response.Content?.ReadAsStringAsync() }
            };
            return HealthCheckResult.FromStatus(status, $"UrlCheck({response.RequestMessage.RequestUri}): status code {response.StatusCode} ({(int)response.StatusCode})", data);
        }

        protected virtual HttpClient GetHttpClient()
            => new HttpClient();
    }
}
