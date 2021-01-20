namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    using System.Threading.Tasks;
    using ViewModels;

    public interface ICampaignService
    {
        Task<Campaign> GetCampaigns(int pageSize, int pageIndex);

        Task<CampaignItem> GetCampaignById(int id);
    }
}