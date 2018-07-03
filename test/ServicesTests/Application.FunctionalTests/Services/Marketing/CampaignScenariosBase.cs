namespace FunctionalTests.Services.Marketing
{
    using System;

    public class CampaignScenariosBase : MarketingScenariosBase
    {
        public static class Get
        {
            public static string Campaigns = CampaignsUrlBase;

            public static string CampaignBy(int id)
                => $"{CampaignsUrlBase}/{id}";

            public static string UserCampaignsByUserId()
                => $"{CampaignsUrlBase}/user";
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