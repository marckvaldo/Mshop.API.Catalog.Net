using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Cache.Category.GetCategory
{
    public class GetCategoryCache : Core.Base.BaseUseCase, IGetCategoryCache
    {
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBuildCacheCategory _buildCache;
        public GetCategoryCache(
            INotification notification, 
            ICategoryCacheRepository categoryCacheRepository, 
            ICategoryRepository categoryRepository,
            IBuildCacheCategory buildCache) : base(notification)
        {
            _categoryCacheRepository = categoryCacheRepository;
            _categoryRepository = categoryRepository;
            _buildCache = buildCache;
        }
            

        public async Task<Result<CategoryModelOutPut>> Handle(GetCategoryCacheInPut request, CancellationToken cancellationToken)
        {
            var category = await _categoryCacheRepository.GetCategoryById(request.Id);

            if(category is null)
            {
                _buildCache.Handle();
                category = await _categoryRepository.GetById(request.Id);  
            }

            if (NotifyErrorIfNull(category, "não foi possivel localizar a categoria na base de dados!"))
                return Result<CategoryModelOutPut>.Error();

            if(!category!.IsValid(Notifications))
                return Result<CategoryModelOutPut>.Error();

           
            return Result<CategoryModelOutPut>.Success(CategoryModelOutPut.FromCategory(category));
        }

    }
}
