using System;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Models
{
    //TO DO: Confirm if this class is not needed, if not, remove it
    //(CDLTLL)
    public class NewOrderViewModel
    {
        public string ShippingCity { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingState { get; set; }

        public string ShippingCountry { get; set; }

        public string CardType { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        public List<OrderItemViewModel> Items { get; set; }

        public NewOrderViewModel()
        {
            Items = new List<OrderItemViewModel>();
        }
    }
}
