using System;

// NOT READY WITH Microsoft.Extensions.Hosting out of ASP.NET Core until .NET Core 2.1
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

namespace Ordering.BackgroundTasksHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    //public class ProgramHelloWorld
    //{
    //    public static async Task Main(string[] args)
    //    {
    //        var builder = new HostBuilder()
    //            .ConfigureServices((hostContext, services) =>
    //            {
    //                services.AddScoped<IHostedService, MyServiceA>();
    //                services.AddScoped<IHostedService, MyServiceB>();
    //            });

    //        await builder.RunConsoleAsync();
    //    }
    //}


}
