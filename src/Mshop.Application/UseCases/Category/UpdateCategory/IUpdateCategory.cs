using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.UpdateCategory
{
    public interface IUpdateCategory : IRequestHandler<UpdateCategoryInPut, Result<CategoryModelOutPut>>
    {
        Task<Result<CategoryModelOutPut>> Handle(UpdateCategoryInPut request, CancellationToken cancellationToken);
    }
}
