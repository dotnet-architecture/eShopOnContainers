using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.eShopOnContainers.Services.Basket.API
{
    internal class TestHttpResponseTrailersFeature : IHttpResponseTrailersFeature
    {
        public IHeaderDictionary Trailers { get; set; }
    }
}