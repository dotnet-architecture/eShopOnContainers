using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CouponService : ICouponService
    {
        public Basket Apply(Basket basket)
        {
            //Basket updatedBasket = new Basket();
            //updatedBasket.BuyerId = basket.BuyerId;
            //// Todo: Stub for now, should reach out to a coupon microservice
            //if (basket.CouponCode == "SM360")
            //{
            //    foreach (BasketItem item in basket.Items)
            //    {
            //        item.UnitPrice = item.UnitPrice - (item.UnitPrice * (decimal)0.1);
            //    }
            //}
            //else
            //{ 
                throw new System.NotImplementedException(); 
            //}
            //return basket;
        }
    }
}
