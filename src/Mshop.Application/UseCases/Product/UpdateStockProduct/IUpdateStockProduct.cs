using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public interface IUpdateStockProduct : IRequestHandler<UpdateStockProductInPut, Result<ProductModelOutPut>>
    {
        public Task<Result<ProductModelOutPut>> Handle(UpdateStockProductInPut request, CancellationToken cancellationToken);
    }
}
