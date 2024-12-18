using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache
{
    public interface IListProductByCategoryCache : IRequestHandler<ListProductByCategoryCacheInPut, Result<ListProductByCategoryCacheOutPut>>
    {

    }
}
