using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CouponService : ICouponService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _apiClient;
        private readonly ILogger<CouponService> _logger;

        public CouponService(HttpClient httpClient, IOptions<AppSettings> settings, ILogger<CouponService> logger)
        {
            _apiClient = httpClient;
            _settings = settings;
            _logger = logger;
        }
        public async Task<Basket> Apply(Basket basket, string couponCode)
        {
            decimal discount = 0;

            List<BasketItem> Items = new List<BasketItem>();
            // Todo: Stub for now, should reach out to a coupon microservice
            // Coupon for smart hotel 360 provides a 10% discount
            if (couponCode == "SH360")
            {
                foreach (BasketItem Item in basket.Items.Select(item => item).ToList())
                {
                    if (Item.isDiscounted == false)
                    {
                        Items.Add(
                        new BasketItem
                        {
                            Id = Item.Id,
                            ProductId = Item.ProductId,
                            ProductName = Item.ProductName,
                            UnitPrice = Item.UnitPrice * (decimal)(1 - 0.1),
                            OldUnitPrice = Item.UnitPrice,
                            Quantity = Item.Quantity,
                            PictureUrl = Item.PictureUrl,
                            isDiscounted = true
                        });

                    }
                }
            }
            else
            {
                foreach (BasketItem Item in basket.Items.Select(item => item).ToList())
                {
                    Items.Add(
                        new BasketItem
                        {
                            Id = Item.Id,
                            ProductId = Item.ProductId,
                            ProductName = Item.ProductName,
                            UnitPrice = Item.UnitPrice,
                            OldUnitPrice = Item.OldUnitPrice,
                            Quantity = Item.Quantity,
                            PictureUrl = Item.PictureUrl,
                            isDiscounted = Item.isDiscounted
                        });
                }
            }
            Basket basketUpdate = new Basket
            {
                BuyerId = basket.BuyerId,
                Items = Items
            };
            return basketUpdate;
        }
    }
}
