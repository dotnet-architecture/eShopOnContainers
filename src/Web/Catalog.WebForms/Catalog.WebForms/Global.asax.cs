using Autofac;
using eShopOnContainers.Core.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Microsoft.eShopOnContainers.Catalog.WebForms
{
    public class Global : HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register Containers:
            var settings= WebConfigurationManager.AppSettings;
            var useFake = settings["usefake"];
            bool fake = useFake == "true";
            var builder = new ContainerBuilder();
            if (fake)
            {
                builder.RegisterType<CatalogMockService>()
                    .As<ICatalogService>();
            } else {
                builder.RegisterType<CatalogMockService>()
                    .As<ICatalogService>();
            }
            var container = builder.Build();
            Application.Add("container", container);
        }
    }
}