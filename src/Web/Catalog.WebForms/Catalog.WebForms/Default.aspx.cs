using Autofac;
using Autofac.Core;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.eShopOnContainers.Catalog.WebForms
{
    public partial class _Default : Page
    {
        private ICatalogService catalog;

        protected _Default() { }

        public _Default(ICatalogService catalog)
        {
            this.catalog = catalog;
        }

        protected override void OnLoad(EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(LoadCatalogDataAsync));

            base.OnLoad(e);
        }

        private async Task LoadCatalogDataAsync()
        {
            var collection = await catalog?.GetCatalogAsync();
            catalogList.DataSource = collection;
            catalogList.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}