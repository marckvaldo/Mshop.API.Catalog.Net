using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.CreateProducts
{
    public interface ICreateProduct : IRequestHandler<CreateProductInPut, Result<ProductModelOutPut>>
    {
        public Task<Result<ProductModelOutPut>> Handle(CreateProductInPut categoryInput, CancellationToken cancellationToken);
    }
}
