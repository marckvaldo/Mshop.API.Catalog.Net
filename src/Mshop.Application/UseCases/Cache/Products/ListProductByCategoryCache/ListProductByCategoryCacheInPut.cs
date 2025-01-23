using MediatR;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using Mshop.Core.DomainObject;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache
{
    public class ListProductByCategoryCacheInPut : PaginatedInPut, IRequest<Result<ListProductByCategoryCacheOutPut>>
    {
        public Guid CategoryId = Guid.Empty;
        public ListProductByCategoryCacheInPut(int page, int perPage, string search, string sort, SearchOrder dir, Guid CategoryId) : 
            base(page, perPage, search, sort, dir)
        {
            this.CategoryId = CategoryId;
        }

        public ListProductByCategoryCacheInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {

        }
    }
}
