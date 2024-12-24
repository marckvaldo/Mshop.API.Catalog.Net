using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;


namespace Mshop.Application.Service
{
    public class BuildCacheCategory : IBuildCacheCategory
    {
        private ICategoryCacheRepository _categoryCacheRepository;
        private ICategoryRepository _categoryRepository;

        public BuildCacheCategory(ICategoryCacheRepository categoryCacheRepository, ICategoryRepository categoryRepository)
        {
            _categoryCacheRepository = categoryCacheRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task Handle()
        {
            var expirationDate = DateTime.UtcNow.AddHours(1);

            var categories = await _categoryRepository.Filter(x=>x.IsActive == true);

            foreach (var category in categories)
            {
                await _categoryCacheRepository.Create(category, expirationDate, CancellationToken.None);
            }
        }
    }
}
