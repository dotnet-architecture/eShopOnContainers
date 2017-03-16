using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eShopOnContainers.Catalog.WebForms.Startup))]
namespace eShopOnContainers.Catalog.WebForms
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
