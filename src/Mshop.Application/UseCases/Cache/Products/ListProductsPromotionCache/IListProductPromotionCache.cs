using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache
{
    public interface IListProductPromotionCache : IRequestHandler<ListProductPromotionCacheInPut, Result<ListProductPromotionCacheOutPut>>
    {
    }
}
