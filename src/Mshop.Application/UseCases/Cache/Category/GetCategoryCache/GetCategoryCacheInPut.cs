using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Category.GetCategory
{
    public class GetCategoryCacheInPut : IRequest<Result<CategoryModelOutPut>>
    {
        public GetCategoryCacheInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
