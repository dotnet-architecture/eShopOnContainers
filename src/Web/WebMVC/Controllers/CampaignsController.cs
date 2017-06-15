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
            //var campaignList = await _campaignService.GetCampaigns();

            var campaignList = new List<Campaign>
            {
                new Campaign
                {
                    Id = 1,
                    Name = "NameTest1",
                    Description = "DescriptionTest1",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/1/pic"
                },
                new Campaign
                {
                    Id = 2,
                    Name = "NameTest2",
                    Description = "DescriptionTest2",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/2/pic"
                },
                new Campaign
                {
                    Id = 3,
                    Name = "NameTest3",
                    Description = "DescriptionTest3",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/3/pic"
                },
                new Campaign
                {
                    Id = 4,
                    Name = "NameTest4",
                    Description = "DescriptionTest4",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/4/pic"
                },
                new Campaign
                {
                    Id = 5,
                    Name = "NameTest5",
                    Description = "DescriptionTest5",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/5/pic"
                },
                new Campaign
                {
                    Id = 6,
                    Name = "NameTest6",
                    Description = "DescriptionTest6",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://localhost:5110/api/v1/campaigns/6/pic"
                }
            };

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