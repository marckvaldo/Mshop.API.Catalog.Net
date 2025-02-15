using Mshop.Application.Common;
using Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Domain.Contract.Services;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using DomainMessage = Mshop.Core.Message;

namespace Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache
{
    public class ListProductPromotionCache : BaseUseCase, IListProductPromotionCache
    {
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;
        private IBuildCacheProduct _buildCache;
        public ListProductPromotionCache(
            IProductRepository productRepository,
            IProductCacheRepository productCacheRepository,
            IBuildCacheProduct buildCache,
            DomainMessage.INotification notification) : base(notification)
        {
            _productCacheRepository = productCacheRepository;
            _productRepository = productRepository;
            _buildCache = buildCache;
        }

        public async Task<Result<ListProductPromotionCacheOutPut>> Handle(ListProductPromotionCacheInPut request, CancellationToken cancellationToken)
        {

            var result = await _productCacheRepository.FilterPaginatedQuery(request, Guid.Empty, true, cancellationToken);

            if (result is null)
            {
                _buildCache.Handle();
                result = await _productRepository.FilterPaginatedQuery(request, Guid.Empty, true, cancellationToken);
            }

            if (NotifyErrorIfNull(result, "não foi possivel localizar os produtos em promoção na base de dados!"))
                return Result<ListProductPromotionCacheOutPut>.Error();

            return Result<ListProductPromotionCacheOutPut>.Success(
                new ListProductPromotionCacheOutPut(
                    result.CurrentPage,
                    result.PerPage,
                    result.Total,
                    result.Itens.Select(d=> ProductModelOutPut.FromProduct(d)).ToList())
                );
        }

    }
}
