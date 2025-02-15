using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using DomainMessage = Mshop.Core.Message;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using Mshop.Domain.Contract.Services;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public class ListCategoriesCache : BaseUseCase, IListCategoriesCache
    {
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBuildCacheCategory _buildCache;

        public ListCategoriesCache(
            ICategoryCacheRepository categoryCacheRepository,
            ICategoryRepository categoryRepository,
            IBuildCacheCategory buildCache,
            DomainMessage.INotification notification) : base(notification)
        {
            _categoryCacheRepository = categoryCacheRepository;
            _categoryRepository = categoryRepository;
            _buildCache = buildCache;
        }

        public async Task<Result<ListCategoriesCacheOutPut>> Handle(ListCategoriesCacheInPut request, CancellationToken cancellationToken)
        {
            if (request.OrderBy is null)
                request.OrderBy = "";

            if (request.Search is null)
                request.Search = "";

            var categories = await _categoryCacheRepository.FilterPaginated(request, cancellationToken);
            if (categories is null)
            {
                _buildCache.Handle();

                categories = await _categoryRepository.FilterPaginated(request, cancellationToken);
            }

            if (NotifyErrorIfNull(categories, "não foi possivel localizar as categorias na base de dados!"))
                return Result<ListCategoriesCacheOutPut>.Error();

            return Result<ListCategoriesCacheOutPut>.Success(
                new ListCategoriesCacheOutPut(
                    categories.CurrentPage,
                    categories.PerPage,
                    categories.Total,
                    categories.Itens.Select(d=> CategoryModelOutPut.FromCategory(d)).ToList())
                );
        }
    }
}
