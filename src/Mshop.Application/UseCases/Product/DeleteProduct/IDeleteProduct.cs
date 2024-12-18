using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.DeleteProduct
{
    public interface IDeleteProduct : IRequestHandler<DeleteProductInPut, Result<ProductModelOutPut>>
    {
        public Task<Result<ProductModelOutPut>> Handle(DeleteProductInPut request, CancellationToken cancellationToken);
    }
}
