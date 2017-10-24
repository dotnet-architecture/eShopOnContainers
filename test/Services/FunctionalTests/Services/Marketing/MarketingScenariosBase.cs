namespace FunctionalTests.Services.Marketing
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class MarketingScenariosBase
    {
        public static string CampaignsUrlBase => "api/v1/campaigns";

        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Marketing");
            webHostBuilder.UseStartup<MarketingTestsStartup>();                  

            return new TestServer(webHostBuilder);
        }
    }
}