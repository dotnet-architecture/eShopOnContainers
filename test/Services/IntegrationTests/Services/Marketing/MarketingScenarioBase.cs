namespace IntegrationTests.Services.Marketing
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using System.IO;

    public class MarketingScenarioBase
    {
        private const string _campaignsUrlBase = "api/v1/campaigns";

        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Marketing");
            webHostBuilder.UseStartup<MarketingTestsStartup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string Campaigns = _campaignsUrlBase;

            public static string CampaignBy(int id)
                => $"{_campaignsUrlBase}/{id}";
        }

        public static class Post
        {
            public static string AddNewCampaign = _campaignsUrlBase;
        }

        public static class Put
        {
            public static string CampaignBy(int id)
                => $"{_campaignsUrlBase}/{id}";
        }

        public static class Delete
        {
            public static string CampaignBy(int id)
                => $"{_campaignsUrlBase}/{id}";
        }
    }
}