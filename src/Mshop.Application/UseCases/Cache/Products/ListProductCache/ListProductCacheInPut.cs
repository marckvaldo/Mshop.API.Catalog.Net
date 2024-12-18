using MediatR;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using Mshop.Core.DomainObject;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCacheInPut : PaginatedInPut, IRequest<Result<ListProductCacheOutPut>>
    {
        public bool onlyProductsOnSale = false;
        public ListProductCacheInPut(int page, int perPage, string search, string sort, SearchOrder dir, bool onlyProductsOnSale) : base(page, perPage, search, sort, dir)
        {
            this.onlyProductsOnSale = onlyProductsOnSale;
        }
    }
}
