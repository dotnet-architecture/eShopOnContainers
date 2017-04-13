using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http
{
    public interface IHttpClient
    {
        HttpClient Inst { get; }
        Task<string> GetStringAsync(string uri);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item);
        Task<HttpResponseMessage> DeleteAsync(string uri);
    }
}
