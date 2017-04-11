using eShopOnContainers.Core.Models.Basket;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace eShopOnContainers.Core.Helpers
{
    public static class ServicesHelper
    {
        private static Regex IpRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

        public static void FixCatalogItemPictureUri(IEnumerable<CatalogItem> catalogItems)
        {
            if (catalogItems == null)
            {
                return;
            }

            try
            {
                if (!ViewModelLocator.UseMockService
                    && Settings.UrlBase != GlobalSetting.DefaultEndpoint)
                {
                    foreach (var catalogItem in catalogItems)
                    {
                        MatchCollection serverResult = IpRegex.Matches(catalogItem.PictureUri);
                        MatchCollection localResult = IpRegex.Matches(Settings.UrlBase);

                        if (serverResult.Count != -1 && localResult.Count != -1)
                        {
                            var serviceIp = serverResult[0].Value;
                            var localIp = localResult[0].Value;

                            catalogItem.PictureUri = catalogItem.PictureUri.Replace(serviceIp, localIp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void FixBasketItemPictureUri(IEnumerable<BasketItem> basketItems)
        {
            if (basketItems == null)
            {
                return;
            }

            try
            {
                if (!ViewModelLocator.UseMockService
                    && Settings.UrlBase != GlobalSetting.DefaultEndpoint)
                {
                    foreach (var basketItem in basketItems)
                    {
                        MatchCollection serverResult = IpRegex.Matches(basketItem.PictureUrl);
                        MatchCollection localResult = IpRegex.Matches(Settings.UrlBase);

                        if (serverResult.Count != -1 && localResult.Count != -1)
                        {
                            var serviceIp = serverResult[0].Value;
                            var localIp = localResult[0].Value;
                            basketItem.PictureUrl = basketItem.PictureUrl.Replace(serviceIp, localIp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
