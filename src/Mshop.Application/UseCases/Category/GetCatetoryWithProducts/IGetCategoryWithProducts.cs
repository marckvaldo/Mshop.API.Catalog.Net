using MediatR;
using Mshop.Application.UseCases.Category.GetCatetoryWithProducts;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.GetCatetoryWithProducts.GetCatetory
{
    public interface IGetCategoryWithProducts : IRequestHandler<GetCategoryWithProductsInPut, Result<GetCategoryWithProductsOutPut>>
    {
        Task<Result<GetCategoryWithProductsOutPut>> Handle(GetCategoryWithProductsInPut request, CancellationToken cancellationToken);
    }
}
