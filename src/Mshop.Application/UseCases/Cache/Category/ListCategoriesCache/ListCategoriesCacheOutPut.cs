using Mshop.Application.Common;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public class ListCategoriesCacheOutPut : PaginatedListOutPut<CategoryModelOutPut>, IModelOutPut
    {
        public ListCategoriesCacheOutPut(int currentPage, int perPage, int total, IReadOnlyList<CategoryModelOutPut> data) :
            base(currentPage, perPage, total, data)
        {
        }
    }
}
