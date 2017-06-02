namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;
    using System.Collections.Generic;

    [Route("api/v1/[controller]")]
    [Authorize]
    public class CampaignsController : Controller
    {
        private readonly MarketingContext _context;

        public CampaignsController(MarketingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaignList = await _context.Campaigns
                .Include(c => c.Rules)
                .ToListAsync();

            var campaignDtoList = CampaignModelListToDtoList(campaignList);

            return Ok(campaignDtoList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.Rules)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (campaign is null)
            {
                return NotFound();
            }

            var campaignDto = CampaignModelToDto(campaign);

            return Ok(campaignDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignDTO campaign)
        {
            if (campaign is null)
            {
                return BadRequest();
            }

            var campaingToCreate = CampaignDtoToModel(campaign);

            await _context.Campaigns.AddAsync(campaingToCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignById), new { id = campaingToCreate.Id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody]CampaignDTO campaign)
        {
            if (id < 1 || campaign is null)
            {
                return BadRequest();
            }

            var campaignToUpdate = await _context.Campaigns.FindAsync(id);
            if (campaignToUpdate is null)
            {
                return NotFound();
            }

            campaignToUpdate.Description = campaign.Description;
            campaignToUpdate.From = campaign.From;
            campaignToUpdate.To = campaign.To;

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



        private List<CampaignDTO> CampaignModelListToDtoList(List<Campaign> campaignList)
        {
            var campaignDtoList = new List<CampaignDTO>();

            campaignList.ForEach(campaign => campaignDtoList
                .Add(CampaignModelToDto(campaign)));

            return campaignDtoList;
        }

        private CampaignDTO CampaignModelToDto(Campaign campaign)
        {
            var campaignDto = new CampaignDTO
            {
                Description = campaign.Description,
                From = campaign.From,
                To = campaign.To,
                Url = campaign.Url,
            };

            campaign.Rules.ForEach(c =>
            {
                switch ((RuleTypeEnum)c.RuleTypeId)
                {
                    case RuleTypeEnum.UserLocationRule:
                        var userLocationRule = c as UserLocationRule;
                        campaignDto.Rules.Add(new RuleDTO
                        {
                            LocationId = userLocationRule.LocationId,
                            RuleTypeId = userLocationRule.RuleTypeId,
                            Description = userLocationRule.Description
                        });
                        break;
                }
            });

            return campaignDto;
        }

        private Campaign CampaignDtoToModel(CampaignDTO campaignDto)
        {
            var campaingModel = new Campaign
            {
                Description = campaignDto.Description,
                From = campaignDto.From,
                To = campaignDto.To,
                Url = campaignDto.Url
            };

            campaignDto.Rules.ForEach(c =>
            {
                switch (c.RuleType)
                {
                    case RuleTypeEnum.UserLocationRule:
                        campaingModel.Rules.Add(new UserLocationRule
                        {
                            LocationId = c.LocationId.Value,
                            RuleTypeId = (int)c.RuleType,
                            Description = c.Description,
                            Campaign = campaingModel
                        });
                        break;
                }
            });

            return campaingModel;
        }

    }
}