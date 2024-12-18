using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Category.ListCategorys
{
    public class ListCategoryOutPut : PaginatedListOutPut<CategoryModelOutPut>, IModelOutPut
    {
        public ListCategoryOutPut(
            int page, 
            int perPage, 
            int total, 
            IReadOnlyList<CategoryModelOutPut> itens) 
            : base(page, perPage, total, itens)
        {

        }
    }
}
