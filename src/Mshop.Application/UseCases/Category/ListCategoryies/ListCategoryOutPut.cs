using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Category.ListCategories
{
    public class ListCategoryOutPut : PaginatedListOutPut<CategoryModelOutPut>, IModelOutPut
    {
        public ListCategoryOutPut(
            int currentPage, 
            int perPage, 
            int total, 
            IReadOnlyList<CategoryModelOutPut> data) 
            : base(currentPage, perPage, total, data)
        {

        }
    }
}
