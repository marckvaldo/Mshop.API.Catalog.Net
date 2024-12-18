using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.GetCategory
{
    public interface IGetCategory : IRequestHandler<GetCategoryInPut, Result<CategoryModelOutPut>>
    {
        Task<Result<CategoryModelOutPut>> Handle(GetCategoryInPut id, CancellationToken cancellationToken);
    }
}
