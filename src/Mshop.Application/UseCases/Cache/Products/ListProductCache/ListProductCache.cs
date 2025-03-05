using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using DomainMessage = Mshop.Core.Message;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCache : BaseUseCase, IListProductCache
    {
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;
        private IBuildCacheProduct _buildCache;
        public ListProductCache(
            IProductRepository productRepository,
            IProductCacheRepository productCacheRepository,
            IBuildCacheProduct buildCache,
            DomainMessage.INotification notification) : base(notification)
        {
            _productCacheRepository = productCacheRepository;
            _productRepository = productRepository;
            _buildCache = buildCache;
        }

        public async Task<Result<ListProductCacheOutPut>> Handle(ListProductCacheInPut request, CancellationToken cancellationToken)
        {
            var produts = await _productCacheRepository.FilterPaginatedQuery(request, request.CategoryId, request.OnlyPromotion, cancellationToken);

            if (produts is null)
            {
                _buildCache.Handle();
                produts = await _productRepository.FilterPaginatedQuery(request, request.CategoryId, request.OnlyPromotion, cancellationToken);
            }

            if (NotifyErrorIfNull(produts, "não foi possivel localizar as categorias na base de dados!"))
                return Result<ListProductCacheOutPut>.Error();

            return Result<ListProductCacheOutPut>.Success(
                new ListProductCacheOutPut(
                    produts.CurrentPage,
                    produts.PerPage,
                    produts.Total,
                    produts.Data.Select(d => ProductModelOutPut.FromProduct(d)).ToList())
                );
        }
    }
}
