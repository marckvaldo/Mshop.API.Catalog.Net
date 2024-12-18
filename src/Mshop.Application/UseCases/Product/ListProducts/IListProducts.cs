using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public interface IListProducts : IRequestHandler<ListProductInPut, Result<ListProductsOutPut>>
    {
        public Task<Result<ListProductsOutPut>> Handle(ListProductInPut request, CancellationToken cancellation);
    }
}
