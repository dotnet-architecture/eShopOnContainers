using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eShopOnContainers.Catalog.WebForms
{
    public partial class EditCatalogItem : System.Web.UI.Page
    {
        private ICatalogService catalog;

        public IEnumerable<CatalogBrand> Brands;
        public IEnumerable<CatalogType> Types;

        protected EditCatalogItem() { }

        public EditCatalogItem(ICatalogService catalog)
        {
            this.catalog = catalog;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // The id parameter should match the DataKeyNames value set on the control
        // or be decorated with a value provider attribute, e.g. [QueryString]int id
        public async Task<CatalogItem> GetCatalogItemAsync([QueryString]int? id)
        {
            if (id.HasValue)
            {
                var itemToEdit = await catalog?.GetCatalogItemAsync(id.ToString());
                return itemToEdit;
            }
            else
            {
                EditCatalogItemForm.ChangeMode(FormViewMode.Insert);
                return new CatalogItem();
            }
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public async Task UpdateCatalogItemAsync(int id)
        {
            CatalogItem item = await catalog?.GetCatalogItemAsync(id.ToString());
            if (item == null)
            {
                // The item wasn't found
                ModelState.AddModelError("", String.Format("Item with id {0} was not found", id));
                return;
            }
            if (TryUpdateModel(item) && (ModelState.IsValid))
            {
                await catalog?.UpdateCatalogItemAsync(item);
                Response.Redirect("~");
            }
        }

        public async Task InsertCatalogItemAsync()
        {
            var item = new eShopOnContainers.Core.Models.Catalog.CatalogItem();
            TryUpdateModel(item);
            if (ModelState.IsValid)
            {
                // Save changes here
                await catalog?.CreateCatalogItemAsync(item);
                Response.Redirect("~");
            }
        }

        public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync()
        {
            var brands = await catalog?.GetCatalogBrandAsync();
            return brands.AsEnumerable();
        }

        public async Task<IEnumerable<CatalogType>> GetTypesAsync()
        {
            var types = await catalog?.GetCatalogTypeAsync();
            return types.AsEnumerable();
        }
    }
}