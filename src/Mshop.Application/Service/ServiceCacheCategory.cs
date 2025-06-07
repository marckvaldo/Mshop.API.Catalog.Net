using Microsoft.Extensions.DependencyInjection;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;


namespace Mshop.Application.Service
{
    public class ServiceCacheCategory : IServiceCacheCategory
    {
        private IServiceProvider _serviceProvider;

        public ServiceCacheCategory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task ExecuteInScopeAsync(Func<ICategoryRepository, ICategoryCacheRepository, IConfigCacheRespository, Task> operation)
        {
            using var scope = _serviceProvider.CreateScope();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var categoryCacheRepository = scope.ServiceProvider.GetRequiredService<ICategoryCacheRepository>();
            var configCacheRepository = scope.ServiceProvider.GetRequiredService<IConfigCacheRespository>();

            await operation(categoryRepository, categoryCacheRepository, configCacheRepository);
        }

        private DateTime GetNewExpirationDate() => DateTime.UtcNow.AddMinutes(1);

        public async Task BuildCache()
        {
            using var scope = _serviceProvider.CreateScope();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var categoryCacheRepository = scope.ServiceProvider.GetRequiredService<ICategoryCacheRepository>();
            var configCacheRepository = scope.ServiceProvider.GetRequiredService<IConfigCacheRespository>();    

            var categories = await categoryRepository.Filter(x=>x.IsActive == true);

            foreach (var category in categories)
            {
                await categoryCacheRepository.DeleteById(category, CancellationToken.None);
                await categoryCacheRepository.Create(category, GetNewExpirationDate(), CancellationToken.None);
            }

            await configCacheRepository.ClearCache();
            await configCacheRepository.SetExpirationDate(GetNewExpirationDate());
        }

        public async Task AddCategory(Guid id, CancellationToken cancellationToken)
        {
            await ExecuteInScopeAsync(async (categoryRepository, categoryCacheRepository, configCacheRepository) =>
            {
                var expirationDate = await configCacheRepository.GetExpirationDate() ?? GetNewExpirationDate();
                var categoryCache = await categoryCacheRepository.GetById(id);
                if (categoryCache is not null)
                    await categoryCacheRepository.DeleteById(categoryCache, cancellationToken);

                var categoryDb = await categoryRepository.GetById(id);
                if (categoryDb is null)
                    return;

                await categoryCacheRepository.Create(categoryDb, expirationDate, cancellationToken);
            });
        }

        public async Task RemoveCategory(Guid id, CancellationToken cancellationToken)
        {
            await ExecuteInScopeAsync(async (categoryRepository, categoryCacheRepository, configCacheRepository) =>
            {
                var categoryCache = await categoryCacheRepository.GetById(id);
                if (categoryCache is null) return;

                await categoryCacheRepository.DeleteById(categoryCache, cancellationToken);
            });
        }

        public async Task UpdateCategory(Guid id, CancellationToken cancellationToken)
        {
            await ExecuteInScopeAsync(async (categoryRepository, categoryCacheRepository, configCacheRepository) =>
            {
                var expirationDate = await configCacheRepository.GetExpirationDate() ?? GetNewExpirationDate();

                var categoryCache = await categoryCacheRepository.GetById(id);
                if (categoryCache is not null) 
                    await categoryCacheRepository.DeleteById(categoryCache, cancellationToken);

                var category = await categoryRepository.GetById(id);
                if (category is null) return;

                await categoryCacheRepository.Create(category, expirationDate, cancellationToken);
            });
        }
    }
}
