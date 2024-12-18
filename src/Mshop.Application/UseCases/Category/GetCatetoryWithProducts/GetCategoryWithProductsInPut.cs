using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.GetCatetoryWithProducts
{
    public class GetCategoryWithProductsInPut : IRequest<Result<GetCategoryWithProductsOutPut>>
    {
        public GetCategoryWithProductsInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
