using eShopOnContainers.Core.Services.IPAddress;
using eShopOnContainers.Droid.Services.IPAddress;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(IPAddressService))]
namespace eShopOnContainers.Droid.Services.IPAddress
{
    public class IPAddressService : IIPAddressService
    {
        public string GetIPAddress()
        {
            System.Net.IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}