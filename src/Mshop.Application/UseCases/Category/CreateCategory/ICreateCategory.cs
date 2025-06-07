using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.CreateCategory
{
    public interface ICreateCategory : IRequestHandler<CreateCategoryInPut, Result<CategoryModelOutPut>>
    {
        //Task<Result<CategoryModelOutPut>> BuildCache(CreateCategoryInPut request, CancellationToken cancellation);
    }
}
