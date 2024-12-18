using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.DeleteCategory
{
    public interface IDeleteCategory : IRequestHandler<DeleteCategoryInPut, Result<CategoryModelOutPut>>
    {
        Task<Result<CategoryModelOutPut>> Handle(DeleteCategoryInPut request, CancellationToken cancellationToken);
    }
}
