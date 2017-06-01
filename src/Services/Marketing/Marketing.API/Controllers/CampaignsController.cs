namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.EntityFrameworkCore;

    [Route("api/v1/[controller]")]
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

            return Ok(campaignList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.Rules)
                .SingleAsync(c => c.Id == id);

            if (campaign is null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignById), new { id = campaign.Id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody]Campaign campaign)
        {
            var campaignToUpdate = await _context.Campaigns.FindAsync(id);
            if (campaign is null)
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
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign is null)
            {
                return NotFound();
            }

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}