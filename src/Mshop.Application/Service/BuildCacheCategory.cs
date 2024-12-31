using Microsoft.Extensions.DependencyInjection;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;


namespace Mshop.Application.Service
{
    public class BuildCacheCategory : IBuildCacheCategory
    {
        //private ICategoryCacheRepository _categoryCacheRepository;
        //private ICategoryRepository _categoryRepository;
        private IServiceProvider _serviceProvider;

        public BuildCacheCategory(IServiceProvider serviceProvider)
        {
            //_categoryCacheRepository = categoryCacheRepository;
            //_categoryRepository = categoryRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task Handle()
        {
            using var scope = _serviceProvider.CreateScope();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var categoryCacheRepository = scope.ServiceProvider.GetRequiredService<ICategoryCacheRepository>();

            var expirationDate = DateTime.UtcNow.AddHours(1);

            var categories = await categoryRepository.Filter(x=>x.IsActive == true);

            foreach (var category in categories)
            {
                await categoryCacheRepository.Create(category, expirationDate, CancellationToken.None);
            }
        }
    }
}
