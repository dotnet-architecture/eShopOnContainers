using Microsoft.Extensions.Options;
using OcelotApiGw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OcelotApiGw.Services
{
    public class SettingService : ISettingService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceFabricSettings _serviceFabricSettings;

        public SettingService(HttpClient httpClient, IOptions<ServiceFabricSettings> serviceFabricSettings)
        {
            _httpClient = httpClient;
            _serviceFabricSettings = serviceFabricSettings.Value;
        }

        public string GetConfiguration()
        {
            var strings = new List<string>();

            var tasks = new Func<Task>[]
            {
               async () => {
                   var configuration = await _httpClient.GetStringAsync(_serviceFabricSettings.UrlMobilMarketing);
                   strings.Add(configuration);
               },
               async () => {
                   var configuration = await _httpClient.GetStringAsync(_serviceFabricSettings.UrlMobilShopping);
                   strings.Add(configuration);
               },
               async () => {
                   var configuration = await _httpClient.GetStringAsync(_serviceFabricSettings.UrlWebMarketing);
                   strings.Add(configuration);
               },
               async () => {
                   var configuration = await _httpClient.GetStringAsync(_serviceFabricSettings.UrlWebShopping);
                   strings.Add(configuration);
               }
            };

            Task.WaitAll(tasks.Select(task => task()).ToArray());

            return string.Join(',', strings);
        }
    }
}
