namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using Infrastructure.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using Microsoft.Extensions.Options;

    [Route("api/v1/[controller]")]
    [Authorize]
    public class CampaignsController : Controller
    {
        private readonly MarketingContext _context;
        private readonly MarketingSettings _settings;
        private readonly IMarketingDataRepository _marketingDataRepository;

        public CampaignsController(MarketingContext context,
            IMarketingDataRepository marketingDataRepository,
             IOptionsSnapshot<MarketingSettings> settings)
        {
            _context = context;
            _marketingDataRepository = marketingDataRepository;
            _settings = settings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaignList = await _context.Campaigns
                .ToListAsync();

            if (campaignList is null)
            {
                return Ok();
            }

            var campaignDtoList = MapCampaignModelListToDtoList(campaignList);

            return Ok(campaignDtoList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            var campaign = await _context.Campaigns
                .SingleOrDefaultAsync(c => c.Id == id);

            if (campaign is null)
            {
                return NotFound();
            }

            var campaignDto = MapCampaignModelToDto(campaign);

            return Ok(campaignDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignDTO campaignDto)
        {
            if (campaignDto is null)
            {
                return BadRequest();
            }

            var campaign = MapCampaignDtoToModel(campaignDto);

            await _context.Campaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignById), new { id = campaign.Id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] CampaignDTO campaignDto)
        {
            if (id < 1 || campaignDto is null)
            {
                return BadRequest();
            }

            var campaignToUpdate = await _context.Campaigns.FindAsync(id);
            if (campaignToUpdate is null)
            {
                return NotFound();
            }

            campaignToUpdate.Name = campaignDto.Name;
            campaignToUpdate.Description = campaignDto.Description;
            campaignToUpdate.From = campaignDto.From;
            campaignToUpdate.To = campaignDto.To;
            campaignToUpdate.PictureUri = campaignDto.PictureUri;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignById), new { id = campaignToUpdate.Id }, null);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            var campaignToDelete = await _context.Campaigns.FindAsync(id);
            if (campaignToDelete is null)
            {
                return NotFound();
            }

            _context.Campaigns.Remove(campaignToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetCampaignsByUserId(Guid userId)
        {
            var marketingData = await _marketingDataRepository.GetAsync(userId.ToString());

            if (marketingData is null)
            {
                return NotFound();
            }

            var campaignDtoList = new List<CampaignDTO>();

            //Get User Location Campaign
            foreach(var userLocation in marketingData.Locations)
            {
                var userCampaignList = await _context.Rules
                    .OfType<UserLocationRule>()
                    .Include(c => c.Campaign)
                    .Where(c => c.Campaign.From >= DateTime.Now
                    && c.Campaign.To <= DateTime.Now 
                    && c.LocationId == userLocation.LocationId)
                    .Select(c => c.Campaign)
                    .ToListAsync();

                if (userCampaignList != null && userCampaignList.Any())
                {
                    var userCampaignDtoList = MapCampaignModelListToDtoList(userCampaignList);
                    campaignDtoList.AddRange(userCampaignDtoList);
                }
            }

            return Ok(campaignDtoList);
        }


        private List<CampaignDTO> MapCampaignModelListToDtoList(List<Campaign> campaignList)
        {
            var campaignDtoList = new List<CampaignDTO>();

            campaignList.ForEach(campaign => campaignDtoList
                .Add(MapCampaignModelToDto(campaign)));

            return campaignDtoList;
        }

        private CampaignDTO MapCampaignModelToDto(Campaign campaign)
        {
            return new CampaignDTO
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                From = campaign.From,
                To = campaign.To,
                PictureUri = GetUriPlaceholder(campaign.PictureUri)
            };
        }

        private Campaign MapCampaignDtoToModel(CampaignDTO campaignDto)
        {
            return new Campaign
            {
                Id = campaignDto.Id,
                Name = campaignDto.Name,
                Description = campaignDto.Description,
                From = campaignDto.From,
                To = campaignDto.To,
                PictureUri = campaignDto.PictureUri
            };
        }

        private string GetUriPlaceholder(string campaignUri)
        {
            var baseUri = _settings.ExternalCatalogBaseUrl;

            campaignUri = campaignUri.Replace("http://externalcatalogbaseurltobereplaced", baseUri);

            return campaignUri;
        }
    }
}