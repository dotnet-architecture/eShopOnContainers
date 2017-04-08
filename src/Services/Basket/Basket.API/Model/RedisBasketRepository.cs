using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net;

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

        public async Task<IEnumerable<string>> GetUsers()
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
                _logger.LogInformation("Problem persisting the item");
                return null;
            }

            _logger.LogInformation("basket item persisted succesfully");
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
            // TODO: Need to make this more robust. ConnectionMultiplexer.ConnectAsync doesn't like domain names or IPv6 addresses.
            if (IPAddress.TryParse(_settings.ConnectionString, out var ip))
            {
                _redis = await ConnectionMultiplexer.ConnectAsync(ip.ToString());
                _logger.LogInformation($"Connecting to database at {_settings.ConnectionString}");
            }
            else
            {
                // workaround for https://github.com/StackExchange/StackExchange.Redis/issues/410
                var ips = await Dns.GetHostAddressesAsync(_settings.ConnectionString);
                _logger.LogInformation($"Connecting to database {_settings.ConnectionString} at IP {ips.First().ToString()}");
                _redis = await ConnectionMultiplexer.ConnectAsync(ips.First().ToString());
            }
        }
    
    }
}

