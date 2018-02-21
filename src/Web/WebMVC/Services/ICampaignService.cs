namespace Microsoft.eShopOnContainers.WebMVC.Services
{
	using Microsoft.eShopOnContainers.Services.Common.API;
	using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModels;

    public interface ICampaignService
    {
        Task<PaginatedItemsViewModel<CampaignItem>> GetCampaigns(int pageSize, int pageIndex);

        Task<CampaignItem> GetCampaignById(int id);
    }
}