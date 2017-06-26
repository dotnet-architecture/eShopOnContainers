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
    }
}