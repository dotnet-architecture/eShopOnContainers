﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public class RedisBasketRepository : IBasketRepository
    {
        private ILogger<RedisBasketRepository> _logger;
        private BasketSettings _settings;

        private ConnectionMultiplexer _redis;

        public RedisBasketRepository(IOptionsSnapshot<BasketSettings> options, ILoggerFactory loggerFactory)
        {
            _settings = options.Value;
            _logger = loggerFactory.CreateLogger<RedisBasketRepository>();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var database = await GetDatabase();
            return await database.KeyDeleteAsync(id.ToString());
        }

        public async Task<IEnumerable<string>> GetUsersAsync()
        {
            var server = await GetServer();
            
            IEnumerable<RedisKey> data = server.Keys();
            if (data == null)
            {
                return null;
            }
            return data.Select(k => k.ToString());
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var database = await GetDatabase();

            var data = await database.StringGetAsync(customerId.ToString());
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var database = await GetDatabase();

            var created = await database.StringSetAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));
            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetBasketAsync(basket.BuyerId);
        }

        private async Task<IDatabase> GetDatabase()
        {
            if (_redis == null)
            {
                await ConnectToRedisAsync();
            }

            return _redis.GetDatabase();
        }

        private async Task<IServer> GetServer()
        {
            if (_redis == null)
            {
                await ConnectToRedisAsync();
            }
            var endpoint = _redis.GetEndPoints();

            return _redis.GetServer(endpoint.First());
        }

        private async Task ConnectToRedisAsync()
        {  
            var configuration = ConfigurationOptions.Parse(_settings.ConnectionString, true);
            configuration.ResolveDns = true;     
            
            _logger.LogInformation($"Connecting to database {configuration.SslHost}.");
            _redis = await ConnectionMultiplexer.ConnectAsync(configuration);
        }
    }
}
