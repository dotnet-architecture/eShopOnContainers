using System.Threading.Tasks;
using System;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System.Net;
using System.IO;
using System.Net.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public abstract class AbstractIntegrationEventHandler<IIntegrationEvent>
    {
        private static String url = @"http://tenantmanager/";
        private readonly IEventBus _eventBus;

        public async Task<bool> CheckIfCustomised(IntegrationEvent @event)
        {
            Boolean result = Get(@event);
            if (result)
            {
                CustomisationEvent customisationEvent = new CustomisationEvent(1, @event);
                try
                {
                    //_logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", eventMessage.Id, Program.AppName, eventMessage);

                    _eventBus.Publish(customisationEvent);
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

                    throw;
                }
            }

            return result;
        }

        private Boolean Get(IntegrationEvent @event)
        {
            //TODO return true/false
            Console.WriteLine("Making API Call...");
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(url);
                try
                {
                    HttpResponseMessage response = client.GetAsync("api/tenants").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Result: " + result);

                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            return false;
        }
    }

}
