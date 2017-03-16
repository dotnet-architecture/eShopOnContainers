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
        public async Task<CatalogItem> EditCatalogItemForm_GetItem([QueryString]int id)
        {
            // TODO: If null, go into insert mode.
            var itemToEdit = await catalog?.GetCatalogItemAsync(id.ToString());
            return itemToEdit;
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void EditCatalogItemForm_UpdateItem(int id)
        {
            eShopOnContainers.Core.Models.Catalog.CatalogItem item = null;
            // Load the item here, e.g. item = MyDataLayer.Find(id);
            if (item == null)
            {
                // The item wasn't found
                ModelState.AddModelError("", String.Format("Item with id {0} was not found", id));
                return;
            }
            TryUpdateModel(item);
            if (ModelState.IsValid)
            {
                // Save changes here, e.g. MyDataLayer.SaveChanges();

            }
        }

        public void EditCatalogItemForm_InsertItem()
        {
            var item = new eShopOnContainers.Core.Models.Catalog.CatalogItem();
            TryUpdateModel(item);
            if (ModelState.IsValid)
            {
                // Save changes here

            }
        }
    }
}