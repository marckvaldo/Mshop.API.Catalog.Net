using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.UpdateThumb
{
    public interface IUpdateThumb : IRequestHandler<UpdateThumbInPut, Result<ProductModelOutPut>>
    {
        public Task<Result<ProductModelOutPut>> Handle(UpdateThumbInPut request, CancellationToken cancellationToken);
    }
}
