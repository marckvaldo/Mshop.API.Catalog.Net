using Microsoft.Extensions.DependencyInjection;
using Mshop.Infra.Cache.Interface;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Infra.Repository.Cache.ConfigRepository
{
    [Collection("Repository Config Collection")]
    [CollectionDefinition("Repository Config Collection", DisableParallelization = true)]
    public class ConfigRepositoryCacheTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly IConfigCacheRespository _configRepositoryCache;
        private readonly StackExchange.Redis.IDatabase _database;

        public ConfigRepositoryCacheTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _configRepositoryCache = _serviceProvider.GetRequiredService<IConfigCacheRespository>();
            _database = connection.GetDatabase();
        }

        [Fact(DisplayName = nameof(SetExpirationDate_ShouldSaveDateInCache))]
        [Trait("Integration - Infra.Cache", "Config Repositorio")]

        public async Task SetExpirationDate_ShouldSaveDateInCache()
        {
            // Arrange
            var expirationDate = DateTime.UtcNow.AddHours(1);
            // Act
            await _configRepositoryCache.SetExpirationDate(expirationDate);
            var cachedDate = await _configRepositoryCache.GetExpirationDate();
            // Assert
            Assert.NotNull(cachedDate);
            Assert.Equal(expirationDate.ToUniversalTime(), cachedDate.Value.ToUniversalTime());
        }


        [Fact(DisplayName = nameof(GetExpirationDate_ShouldReturnCorrectDate))]
        [Trait("Integration - Infra.Cache", "Config Repositorio")]

        public async Task GetExpirationDate_ShouldReturnCorrectDate()
        {
            // Arrange
            var expirationDate = DateTime.UtcNow.AddHours(1);
            await _configRepositoryCache.SetExpirationDate(expirationDate);
            // Act
            var cachedDate = await _configRepositoryCache.GetExpirationDate();
            // Assert
            Assert.NotNull(cachedDate);
            Assert.Equal(expirationDate.ToUniversalTime(), cachedDate.Value.ToUniversalTime());
        }


        [Fact(DisplayName = nameof(ClearCache_ShouldRemoveExpirationDate))]
        [Trait("Integration - Infra.Cache", "Config Repositorio")]
        public async Task ClearCache_ShouldRemoveExpirationDate()
        {
            // Arrange
            var expirationDate = DateTime.UtcNow.AddHours(1);
            await _configRepositoryCache.SetExpirationDate(expirationDate);
            // Act
            await _configRepositoryCache.ClearCache();
            var cachedDate = await _configRepositoryCache.GetExpirationDate();
            // Assert
            Assert.Null(cachedDate);
        }

        [Fact(DisplayName = nameof(ClearCache_ShouldRemoveExpirationDate))]
        [Trait("Integration - Infra.Cache", "Config Repositorio")]
        public async Task GetExpirationDate_ShouldReturnNull_WhenCacheIsEmpty()
        {
            //arrange
            await _configRepositoryCache.ClearCache();

            // Act
            var cachedDate = await _configRepositoryCache.GetExpirationDate();
            // Assert
            Assert.Null(cachedDate);
        }

        public void Dispose()
        {
            // Limpar o banco de dados Redis após os testes
            _database.KeyDelete("ConfigCache:ExpirationDate");
        }
    }
}
