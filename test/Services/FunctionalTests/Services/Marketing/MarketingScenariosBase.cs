namespace FunctionalTests.Services.Marketing
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using System.IO;

    public class MarketingScenariosBase
    {
        public static string CampaignsUrlBase => "api/v1/campaigns";

        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Marketing");
            webHostBuilder.UseStartup<MarketingTestsStartup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string Campaigns = CampaignsUrlBase;

            public static string CampaignBy(int id)
                => $"{CampaignsUrlBase}/{id}";

            public static string UserCampaignByUserId(System.Guid userId)
                => $"{CampaignsUrlBase}/user/{userId}";
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
