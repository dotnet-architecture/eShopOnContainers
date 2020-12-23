using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WebhookClient;

CreateWebHostBuilder(args).Build().Run();


IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
