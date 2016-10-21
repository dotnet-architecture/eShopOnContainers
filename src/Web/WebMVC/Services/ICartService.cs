using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface ICartService
    {
        void AddItemToOrder(CatalogItem item);
        void RemoveItemFromOrder(Guid itemIdentifier);
        int GetItemCountFromOrderInProgress();
        Task<Order> GetOrderInProgress();
    }
}
