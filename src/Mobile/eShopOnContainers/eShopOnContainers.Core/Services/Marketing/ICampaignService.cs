
namespace eShopOnContainers.Core.Services.Marketing
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Models.Marketing;

    public interface ICampaignService
    {
        Task<ObservableCollection<CampaignItem>> GetAllCampaignsAsync(string userId, string token);

        Task<CampaignItem> GetCampaignByIdAsync(int id, string token);
    }
}