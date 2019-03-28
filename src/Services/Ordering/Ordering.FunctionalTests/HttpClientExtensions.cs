using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;

namespace Ordering.FunctionalTests
{
    static class HttpClientExtensions
    {
        public static HttpClient CreateIdempotentClient(this TestServer server)
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            return client;
        }
    }
}
