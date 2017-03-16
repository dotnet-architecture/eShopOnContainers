using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eShopOnContainers.Catalog.WebForms
{
    public partial class _Default : Page
    {
        private ICatalogService catalog;
        private CatalogItem itemToEdit;

        protected _Default() { }

        public _Default(ICatalogService catalog)
        {
            this.catalog = catalog;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public async Task<IEnumerable<CatalogItem>> catalogList_GetData()
        {
            return await catalog?.GetCatalogAsync();
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public async Task catalogList_DeleteItem(int id)
        {
            //TODO: Call the service.
        }
    }
}