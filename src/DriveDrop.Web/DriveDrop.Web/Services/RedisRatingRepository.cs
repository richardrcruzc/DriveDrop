using DriveDrop.Web.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public class RedisRatingRepository : IRatingRepository
    {
        private ILogger<RedisRatingRepository> _logger;
        private AppSettings _settings;


        private ConnectionMultiplexer _redis;

        public RedisRatingRepository(IOptionsSnapshot<AppSettings> options, ILoggerFactory loggerFactory)
        {
            _settings = options.Value;
            _logger = loggerFactory.CreateLogger<RedisRatingRepository>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var database = await GetDatabase();
            return await database.KeyDeleteAsync(id.ToString());
        }
        public async Task<ReviewModel> GetAsync(int shippingId)
        {

            var database = await GetDatabase();
            var data = await database.StringGetAsync(shippingId.ToString());
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ReviewModel>(data);
        }

        public async Task<ReviewModel> UpdateAsync(ReviewModel rating)
        {
            var database = await GetDatabase();

            var created = await database.StringSetAsync(rating.ShippingId.ToString(), JsonConvert.SerializeObject(rating));
            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetAsync(rating.ShippingId);
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
            var configuration = ConfigurationOptions.Parse(_settings.RediConnectionString, true);
            configuration.ResolveDns = true;
            configuration.AbortOnConnectFail = false;

            ConfigurationOptions co = new ConfigurationOptions()
            {
                SyncTimeout = 500000,
                EndPoints =
            {
                {_settings.RediConnectionString}
            },
                AbortOnConnectFail = false // this prevents that error
            };



            _logger.LogInformation($"Connecting to database {configuration.SslHost}.");
            _redis = await ConnectionMultiplexer.ConnectAsync(co);
        }
    }
}
