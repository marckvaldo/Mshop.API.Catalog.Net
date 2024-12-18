using MediatR;
using Mshop.Application.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Enum.Paginated;


namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductInPut : PaginatedListInput, IRequest<Result<ListProductsOutPut>>
    {
        public ListProductInPut(int page, int perPage, string search, string sort, SearchOrder dir) : base(page, perPage, search, sort, dir)
        {

        }

        public ListProductInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {

        }
    }
}
