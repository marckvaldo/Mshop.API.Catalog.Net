using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public interface IListProductCache : IRequestHandler<ListProductCacheInPut, Result<ListProductCacheOutPut>>
    {

    }
}
