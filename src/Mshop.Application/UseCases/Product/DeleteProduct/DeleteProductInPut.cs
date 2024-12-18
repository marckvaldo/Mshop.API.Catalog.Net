using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.DeleteProduct
{
    public class DeleteProductInPut : IRequest<Result<ProductModelOutPut>>
    {
        public DeleteProductInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
