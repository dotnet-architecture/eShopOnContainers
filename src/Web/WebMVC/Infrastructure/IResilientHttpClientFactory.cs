using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using System;

namespace Microsoft.eShopOnContainers.WebMVC.Infrastructure
{
    public interface IResilientHttpClientFactory
    {
        ResilientHttpClient CreateResilientHttpClient();
    }
}