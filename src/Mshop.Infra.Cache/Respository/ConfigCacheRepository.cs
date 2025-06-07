using Mshop.Infra.Cache.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Cache.Respository
{
    public class ConfigCacheRepository : IConfigCacheRespository
    {
        private readonly IDatabase _database;
        private const string ExpirationKey = "ConfigCache:ExpirationDate";

        public ConfigCacheRepository(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
        }

        public async Task ClearCache()
        {
            await _database.KeyDeleteAsync(ExpirationKey);
        }

        public async Task<DateTime?> GetExpirationDate()
        {
            var expirationValue = await _database.StringGetAsync(ExpirationKey);
            if (!expirationValue.HasValue)
                return null;

            if (DateTime.TryParse(expirationValue, out var expirationDate))
                return expirationDate;

            return null;
        }

        public async Task SetExpirationDate(DateTime expirationDate)
        {
            await _database.StringSetAsync(ExpirationKey, expirationDate.ToString("o"), null);
        }
    }
}
