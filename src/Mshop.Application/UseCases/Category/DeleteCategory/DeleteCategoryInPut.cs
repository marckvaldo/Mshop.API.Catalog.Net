using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategoryInPut : IRequest<Result<CategoryModelOutPut>>
    {
        public DeleteCategoryInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; } 


    }
}
