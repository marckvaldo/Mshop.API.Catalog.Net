using MediatR;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using Mshop.Core.DomainObject;
using Mshop.Application.Common;
using Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCacheInPut : PaginatedInPut, IRequest<Result<ListProductCacheOutPut>>
    {
        public bool OnlyPromotion { get; set; }
        public Guid CategoryId { get; set; }

        public ListProductCacheInPut(int page, 
                int perPage, 
                string search, 
                string sort, 
                SearchOrder dir, 
                bool onlyPromotion, 
                Guid categoryId = default) :
            base(page, perPage, search, sort, dir)
        {
            OnlyPromotion = onlyPromotion;
            CategoryId = categoryId;
        }

        public ListProductCacheInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {

        }
    }
}
