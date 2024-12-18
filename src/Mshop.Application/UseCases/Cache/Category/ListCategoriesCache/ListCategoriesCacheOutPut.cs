using Mshop.Application.Common;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public class ListCategoriesCacheOutPut : PaginatedListOutPut<CategoryModelOutPut>, IModelOutPut
    {
        public ListCategoriesCacheOutPut(int page, int perPage, int total, IReadOnlyList<CategoryModelOutPut> itens) :
            base(page, perPage, total, itens)
        {
        }
    }
}
