using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using System.Net.Http;

namespace Microsoft.eShopOnContainers.WebMVC.Infrastructure
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly ILogger<ResilientHttpClient> _logger;

        public ResilientHttpClientFactory(ILogger<ResilientHttpClient> logger) 
            =>_logger = logger;        

        public  ResilientHttpClient CreateResilientHttpClient()        
            => new ResilientHttpClient(_logger);
    }
}
