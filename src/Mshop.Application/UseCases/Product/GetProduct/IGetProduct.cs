using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.GetProduct
{
    public interface IGetProduct : IRequestHandler<GetProductInPut, Result<GetProductOutPut>>
    {
        public Task<Result<GetProductOutPut>> Handle(GetProductInPut request, CancellationToken cancellation);
    }
}
