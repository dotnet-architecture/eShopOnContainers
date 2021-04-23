using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface ICouponService
    {
        Basket Apply(Basket basket);
    }
}
