using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public SettingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public string GetConfiguration()
        {
            var strings = new List<string>();

            var tasks = new Func<Task>[]
            {
               async () => {
                   var res = await _httpClient.GetStringAsync(_configuration.GetValue<string>("MobilMarketing"));
                   strings.Add(res);
               },
               async () => {
                   var res = await _httpClient.GetStringAsync(_configuration.GetValue<string>("MobilShopping"));
                   strings.Add(res);
               },
               async () => {
                   var res = await _httpClient.GetStringAsync(_configuration.GetValue<string>("WebMarketing"));
                   strings.Add(res);
               },
               async () => {
                   var res = await _httpClient.GetStringAsync(_configuration.GetValue<string>("WebShopping"));
                   strings.Add(res);
               }
            };

            Task.WaitAll(tasks.Select(task => task()).ToArray());

            return string.Concat(strings);
        }
    }
}
