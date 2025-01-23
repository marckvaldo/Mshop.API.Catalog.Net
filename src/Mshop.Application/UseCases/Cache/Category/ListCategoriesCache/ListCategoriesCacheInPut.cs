using MediatR;
using Mshop.Core.DomainObject;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;

namespace Mshop.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public class ListCategoriesCacheInPut : PaginatedInPut, IRequest<Result<ListCategoriesCacheOutPut>>
    {
        public ListCategoriesCacheInPut(
            int page,
            int perPage,
            string search,
            string sort,
            SearchOrder dir)
            : base(page, perPage, search, sort, dir)
        {

        }

        public ListCategoriesCacheInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {

        }
    }
}
