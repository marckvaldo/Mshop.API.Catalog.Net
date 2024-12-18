using MediatR;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using Mshop.Core.DomainObject;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache
{
    public class ListProductPromotionCacheInPut : PaginatedInPut, IRequest<Result<ListProductPromotionCacheOutPut>>
    {
        public bool onlyProductsOnSale = false;
        public ListProductPromotionCacheInPut(int page, int perPage, string search, string sort, SearchOrder dir, bool onlyProductsOnSale) : base(page, perPage, search, sort, dir)
        {
            this.onlyProductsOnSale = onlyProductsOnSale;
        }
    }
}
