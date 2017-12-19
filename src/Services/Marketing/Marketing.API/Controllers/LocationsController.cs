namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    [Authorize]
    public class LocationsController : Controller
    {
        private readonly MarketingContext _context;

        public LocationsController(MarketingContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/v1/campaigns/{campaignId:int}/locations/{userLocationRuleId:int}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(UserLocationRuleDTO),(int)HttpStatusCode.OK)]
        public IActionResult GetLocationByCampaignAndLocationRuleId(int campaignId, 
            int userLocationRuleId)
        {
            if (campaignId < 1 || userLocationRuleId < 1)
            {
                return BadRequest();
            }

            var location = _context.Rules
                .OfType<UserLocationRule>()
                .SingleOrDefault(c => c.CampaignId == campaignId && c.Id == userLocationRuleId);

            if (location is null)
            {
                return NotFound();
            }

            var locationDto = MapUserLocationRuleModelToDto(location);

            return Ok(locationDto);
        }

        [HttpGet]
        [Route("api/v1/campaigns/{campaignId:int}/locations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<UserLocationRuleDTO>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllLocationsByCampaignId(int campaignId)
        {
            if (campaignId < 1)
            {
                return BadRequest();
            }

            var locationList = _context.Rules
                .OfType<UserLocationRule>()
                .Where(c => c.CampaignId == campaignId)
                .ToList();

            if(locationList is null)
            {
                return Ok();
            }

            var locationDtoList = MapUserLocationRuleModelListToDtoList(locationList);

            return Ok(locationDtoList);
        }

        [HttpPost]
        [Route("api/v1/campaigns/{campaignId:int}/locations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateLocation(int campaignId, 
            [FromBody] UserLocationRuleDTO locationRuleDto)
        {
            if (campaignId < 1 || locationRuleDto is null)
            {
                return BadRequest();
            }

            var locationRule = MapUserLocationRuleDtoToModel(locationRuleDto);
            locationRule.CampaignId = campaignId;

            await _context.Rules.AddAsync(locationRule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocationByCampaignAndLocationRuleId),
                new { campaignId = campaignId, userLocationRuleId = locationRule.Id }, null);
        }

        [HttpDelete]
        [Route("api/v1/campaigns/{campaignId:int}/locations/{userLocationRuleId:int}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteLocationById(int campaignId, int userLocationRuleId)
        {
            if (campaignId < 1 || userLocationRuleId < 1)
            {
                return BadRequest();
            }

            var locationToDelete = _context.Rules
                .OfType<UserLocationRule>()
                .SingleOrDefault(c => c.CampaignId == campaignId && c.Id == userLocationRuleId);

            if (locationToDelete is null)
            {
                return NotFound();
            }

            _context.Rules.Remove(locationToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private List<UserLocationRuleDTO> MapUserLocationRuleModelListToDtoList(List<UserLocationRule> userLocationRuleList)
        {
            var userLocationRuleDtoList = new List<UserLocationRuleDTO>();

            userLocationRuleList.ForEach(userLocationRule => userLocationRuleDtoList
                .Add(MapUserLocationRuleModelToDto(userLocationRule)));

            return userLocationRuleDtoList;
        }

        private UserLocationRuleDTO MapUserLocationRuleModelToDto(UserLocationRule userLocationRule)
        {
            return new UserLocationRuleDTO
            {
                Id = userLocationRule.Id,
                Description = userLocationRule.Description,
                LocationId = userLocationRule.LocationId
            };
        }

        private UserLocationRule MapUserLocationRuleDtoToModel(UserLocationRuleDTO userLocationRuleDto)
        {
            return new UserLocationRule
            {
                Id = userLocationRuleDto.Id,
                Description = userLocationRuleDto.Description,
                LocationId = userLocationRuleDto.LocationId
            };
        }
    }
}