using MediatR;
using Mshop.Application.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Enum.Paginated;

namespace Mshop.Application.UseCases.Category.ListCategorys
{
    public class ListCategoryInPut : PaginatedListInput, IRequest<Result<ListCategoryOutPut>>
    {
        public ListCategoryInPut(
            int page, 
            int perPage, 
            string search, 
            string sort, 
            SearchOrder dir) 
            : base(page, perPage, search, sort, dir)
        {

        }

        public ListCategoryInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {

        }
    }
}
