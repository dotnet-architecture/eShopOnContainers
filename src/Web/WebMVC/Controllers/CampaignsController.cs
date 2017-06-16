namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.WebMVC.Services;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize]
    public class CampaignsController : Controller
    {
        private ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService) =>
            _campaignService = campaignService;

        public async Task<IActionResult> Index()
        {
            var campaignList = await _campaignService.GetCampaigns();

            return View(campaignList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var campaignDto = await _campaignService.GetCampaignById(id);

            var campaign = new Campaign
            {
                Id = campaignDto.Id,
                Name = campaignDto.Name,
                Description = campaignDto.Description,
                From = campaignDto.From,
                To = campaignDto.To,
                PictureUri = campaignDto.PictureUri
            };

            return View(campaign);
        }
    }
}