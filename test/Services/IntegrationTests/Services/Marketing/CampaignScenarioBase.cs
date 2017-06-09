namespace IntegrationTests.Services.Marketing
{
    public class CampaignScenarioBase : MarketingScenarioBase
    {
        public static class Get
        {
            public static string Campaigns = CampaignsUrlBase;

            public static string CampaignBy(int id)
                => $"{CampaignsUrlBase}/{id}";
        }

        public static class Post
        {
            public static string AddNewCampaign = CampaignsUrlBase;
        }

        public static class Put
        {
            public static string CampaignBy(int id)
                => $"{CampaignsUrlBase}/{id}";
        }

        public static class Delete
        {
            public static string CampaignBy(int id)
                => $"{CampaignsUrlBase}/{id}";
        }
    }
}