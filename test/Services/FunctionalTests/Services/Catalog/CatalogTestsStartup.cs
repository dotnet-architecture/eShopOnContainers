using Microsoft.eShopOnContainers.Services.Catalog.API;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace FunctionalTests.Services.Catalog
{
    public class CatalogTestsStartup : Startup
    {
        public CatalogTestsStartup(IHostingEnvironment env) : base(env)
        {
        }
    }
}
