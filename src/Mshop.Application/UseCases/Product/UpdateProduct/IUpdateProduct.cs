using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.UpdateProduct
{
    public interface IUpdateProduct : IRequestHandler<UpdateProductInPut, Result<ProductModelOutPut>>
    {
        public Task<Result<ProductModelOutPut>> Handle(UpdateProductInPut request, CancellationToken cancellationToken);
    }
}
