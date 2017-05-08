using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services. 
    /// Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private HttpClient _client;
        private PolicyWrap _policyWrapper;
        private ILogger<ResilientHttpClient> _logger;
        //public HttpClient Inst => _client;

        public ResilientHttpClient(Policy[] policies, ILogger<ResilientHttpClient> logger)
        {
            _client = new HttpClient();
            _logger = logger;

            // Add Policies to be applied
            _policyWrapper = Policy.WrapAsync(policies);
        }

        private Task<T> HttpInvoker<T>(Func<Task<T>> action)
        {
            // Executes the action applying all 
            // the policies defined in the wrapper
            return _policyWrapper.ExecuteAsync(() => action());
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            return HttpInvoker(async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                var response = await _client.SendAsync(requestMessage);

                return await response.Content.ReadAsStringAsync();
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {

            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            return HttpInvoker(async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return HttpInvoker(async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                return await _client.SendAsync(requestMessage);
            });
        }

    }
}
