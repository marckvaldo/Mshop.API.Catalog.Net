using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using DomainMessage = Mshop.Core.Message;

namespace Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache
{
    public class ListProductByCategoryCache : BaseUseCase, IListProductByCategoryCache
    {
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;
        private IBuildCacheProduct _buildCache;
        public ListProductByCategoryCache(
            IProductRepository productRepository,
            IProductCacheRepository productCacheRepository,
            IBuildCacheProduct buildCache,
            DomainMessage.INotification notification) : base(notification)
        {
            _productCacheRepository = productCacheRepository;
            _productRepository = productRepository;
            _buildCache = buildCache;
        }

        public async Task<Result<ListProductByCategoryCacheOutPut>> Handle(ListProductByCategoryCacheInPut request, CancellationToken cancellationToken)
        {
            var produts = await _productCacheRepository.FilterPaginatedQuery(request, request.CategoryId, false, cancellationToken);

            if (produts is null)
            {
                _buildCache.Handle();
                produts = await _productRepository.FilterPaginatedQuery(request, request.CategoryId, false, cancellationToken);
            }

            if (NotifyErrorIfNull(produts, "não foi possivel localizar as categorias na base de dados!"))
                return Result<ListProductByCategoryCacheOutPut>.Error();

            return Result<ListProductByCategoryCacheOutPut>.Success(
                new ListProductByCategoryCacheOutPut(
                    produts.CurrentPage,
                    produts.PerPage,
                    produts.Total,
                    produts.Itens.Select(d => ProductModelOutPut.FromProduct(d)).ToList())
                );
        }
    }
}
