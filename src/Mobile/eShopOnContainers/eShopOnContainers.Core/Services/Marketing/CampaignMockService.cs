using System.Collections.Generic;
using System.Linq;

namespace eShopOnContainers.Core.Services.Marketing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Models.Marketing;
    using Xamarin.Forms;

    public class CampaignMockService : ICampaignService
    {
        private readonly ObservableCollection<Campaign> _mockCampaign = new ObservableCollection<Campaign>
        {
            new Campaign
            {
                Id = Common.Common.MockCampaignd01,
                PictureUri = Device.RuntimePlatform != Device.Windows
                    ? "fake_campaign_01.png"
                    : "Assets/fake_campaign_01.png",
                Name = ".NET Bot Black Hoodie 50% OFF",
                Description = "Campaign Description 1",
                From = DateTime.Now,
                To = DateTime.Now.AddDays(7)
            },

            new Campaign
            {
                Id = Common.Common.MockCampaignd02,
                PictureUri = Device.RuntimePlatform != Device.Windows
                    ? "fake_campaign_02.png"
                    : "Assets/fake_campaign_02.png",
                Name = "Roslyn Red T-Shirt 3x2",
                Description = "Campaign Description 2",
                From = DateTime.Now.AddDays(-7),
                To = DateTime.Now.AddDays(14)
            }
        };

        public async Task<ObservableCollection<Campaign>> GetAllCampaignsAsync(string userId, string token)
        {
            await Task.Delay(500);

            return _mockCampaign;
        }

        public async Task<Campaign> GetCampaignByIdAsync(int camapignId, string token)
        {
            await Task.Delay(500);

            return _mockCampaign.First();
        }
    }
}