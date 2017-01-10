using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.ViewModels.Base;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace eShopOnContainers.Core.Helpers
{
    public static class ServicesHelper
    {
        public static void FixCatalogItemPictureUri(IEnumerable<CatalogItem> catalogItems)
        {
            if(catalogItems == null)
            {
                return;
            }

            if (!ViewModelLocator.Instance.UseMockService 
                && Settings.UrlBase != GlobalSetting.DefaultEndpoint)
            {
                foreach (var catalogItem in catalogItems)
                {
                    Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                    MatchCollection serverResult = ip.Matches(catalogItem.PictureUri);
                    MatchCollection localResult = ip.Matches(Settings.UrlBase);

                    if (serverResult.Count != -1 && localResult.Count != -1)
                    {
                        var serviceIp = serverResult[0].Value;
                        var localIp = localResult[0].Value;
                        catalogItem.PictureUri = catalogItem.PictureUri.Replace(serviceIp, localIp);
                    }
                }
            }
        }
    }
}
