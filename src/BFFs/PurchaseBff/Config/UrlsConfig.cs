using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Config
{
    public class UrlsConfig
    {
        public class CatalogOperations
        {
            public static string GetItemById(int id) => $"/api/v1/catalog/items/{id}";
        }

        public class BasketOperations
        {
            public static string GetItemById(string id) => $"/api/v1/basket/{id}";
            public static string UpdateBasket() => $"/api/v1/basket";
        }

        public string Basket { get; set; }
        public string Catalog { get; set; }
    }
}
