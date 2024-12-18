using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.GetProductCache
{
    public class GetProductCacheInPut : IRequest<Result<GetProductCacheOutPut>>
    {
        public GetProductCacheInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
