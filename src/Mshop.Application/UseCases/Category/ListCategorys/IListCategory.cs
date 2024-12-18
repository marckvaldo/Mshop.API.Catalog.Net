using MediatR;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.ListCategorys
{
    public interface IListCategory : IRequestHandler<ListCategoryInPut, Result<ListCategoryOutPut>>
    {
        Task<Result<ListCategoryOutPut>> Handle(ListCategoryInPut request, CancellationToken cancellation);
    }
}
