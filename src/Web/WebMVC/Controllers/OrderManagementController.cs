using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;

namespace WebMVC.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class OrderManagementController : Controller
    {
        private IOrderingService _orderSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        public OrderManagementController(IOrderingService orderSvc, IIdentityParser<ApplicationUser> appUserParser)
        {
            _appUserParser = appUserParser;
            _orderSvc = orderSvc;
        }

        public async Task<IActionResult> Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = await _orderSvc.GetMyOrders(user);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> OrderProcess(string orderId, string actionCode)
        {
            if (OrderProcessAction.Ship.Code == actionCode)
            {
                await _orderSvc.ShipOrder(orderId);
            }

            return RedirectToAction("Index");
        }
    }
}
