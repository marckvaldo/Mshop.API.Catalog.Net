using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.GetProduct
{
    public class GetProductInPut : IRequest<Result<GetProductOutPut>>
    {
        public GetProductInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
