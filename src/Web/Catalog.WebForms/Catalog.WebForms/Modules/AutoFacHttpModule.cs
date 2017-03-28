using Autofac;
using eShopOnContainers.Core.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace Microsoft.eShopOnContainers.Catalog.WebForms.Modules
{
    // Using DI with WebForms is not yet implemented.
    // This implementation has been adapted from this post:
    // https://blogs.msdn.microsoft.com/webdev/2016/10/19/modern-asp-net-web-forms-development-dependency-injection/

    public class AutoFacHttpModule : IHttpModule
    {
        private static IContainer Container => lazyContainer.Value;

        private static Lazy<IContainer> lazyContainer = new Lazy<IContainer>(() => CreateContainer());

        private static IContainer CreateContainer()
        {
            // Configure AutoFac:
            // Register Containers:
            var settings = WebConfigurationManager.AppSettings;
            var useFake = settings["usefake"];
            bool fake = useFake == "true";
            var builder = new ContainerBuilder();
            if (fake)
            {
                builder.RegisterType<CatalogMockService>()
                    .As<ICatalogService>();
            }
            else
            {
                builder.RegisterType<CatalogMockService>()
                    .As<ICatalogService>();
            }
            var container = builder.Build();
            return container;
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (_, __) => InjectDependencies();
        }

        private void InjectDependencies()
        {
            if  (HttpContext.Current.CurrentHandler is Page page)
            {
                // Get the code-behind class that we may have written
                var pageType = page.GetType().BaseType;

                // Determine if there is a constructor to inject, and grab it
                var ctor = (from c in pageType.GetConstructors()
                            where c.GetParameters().Length > 0
                            select c).FirstOrDefault();

                if (ctor != null)
                {
                    // Resolve the parameters for the constructor
                    var args = (from parm in ctor.GetParameters()
                                select Container.Resolve(parm.ParameterType))
                                .ToArray();

                    // Execute the constructor method with the arguments resolved 
                    ctor.Invoke(page, args);
                }

                // Use the Autofac method to inject any properties that can be filled by Autofac
                Container.InjectProperties(page);

            }
        }
    }
}