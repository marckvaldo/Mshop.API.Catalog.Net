using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Category.GetCategory
{
    public interface IGetCategoryCache : IRequestHandler<GetCategoryCacheInPut, Result<CategoryModelOutPut>>
    {

    }
}
