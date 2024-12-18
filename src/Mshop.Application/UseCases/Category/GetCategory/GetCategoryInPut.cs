using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.GetCategory
{
    public class GetCategoryInPut :IRequest<Result<CategoryModelOutPut>>
    {
        public GetCategoryInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
