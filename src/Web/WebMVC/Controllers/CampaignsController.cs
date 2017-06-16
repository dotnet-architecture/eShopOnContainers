namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.WebMVC.Models;
    using Microsoft.eShopOnContainers.WebMVC.Services;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
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
            var campaignDtoList = await _campaignService.GetCampaigns();

            if(campaignDtoList is null)
            {
                return View();
            }

            var campaignList = MapCampaignModelListToDtoList(campaignDtoList);

            return View(campaignList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var campaignDto = await _campaignService.GetCampaignById(id);

            if (campaignDto is null)
            {
                return NotFound();
            }

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

        private List<Campaign> MapCampaignModelListToDtoList(IEnumerable<CampaignDTO> campaignDtoList)
        {
            var campaignList = new List<Campaign>();

            foreach(var campaignDto in campaignDtoList)
            {
                campaignList.Add(MapCampaignDtoToModel(campaignDto));
            }

            return campaignList;
        }

        private Campaign MapCampaignDtoToModel(CampaignDTO campaign)
        {
            return new Campaign
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                From = campaign.From,
                To = campaign.To,
                PictureUri = campaign.PictureUri
            };
        }
    }
}