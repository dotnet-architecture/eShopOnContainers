
namespace eShopOnContainers.Core.Services.Marketing
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Models.Marketing;

    public interface ICampaignService
    {
        Task<ObservableCollection<Campaign>> GetAllCampaignsAsync(string userId, string token);

        Task<Campaign> GetCampaignByIdAsync(int id, string token);
    }
}