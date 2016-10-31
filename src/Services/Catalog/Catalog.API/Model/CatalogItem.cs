namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
{
    using System;

    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUri { get; set; }

        public CatalogItem() { }     
    }
}
