namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.WebMVC.Services;

    [Authorize]
    public class CampaignsController : Controller
    {
        private ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService) =>
            _campaignService = campaignService;

        public IActionResult Index()
        {
            var campaignList = _campaignService.GetCampaigns();
            return View();
        }
    }
}