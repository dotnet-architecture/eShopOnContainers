namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    using Microsoft.eShopOnContainers.WebMVC.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICampaignService
    {
        Task<List<CampaignDTO>> GetCampaigns();
    }
}