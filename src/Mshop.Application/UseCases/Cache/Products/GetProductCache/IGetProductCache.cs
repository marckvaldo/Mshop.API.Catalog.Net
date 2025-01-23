using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.GetProductCache
{
    public interface IGetProductCache : IRequestHandler<GetProductCacheInPut, Result<GetProductCacheOutPut>>
    {
    }
}
