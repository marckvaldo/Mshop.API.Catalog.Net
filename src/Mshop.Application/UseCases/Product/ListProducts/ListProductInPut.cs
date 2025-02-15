using MediatR;
using Mshop.Application.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Enum.Paginated;
using Mshop.Domain.Entity;


namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductInPut : PaginatedListInput, IRequest<Result<ListProductsOutPut>>
    {
        public bool OnlyPromotion { get; }
        public Guid CategoryId { get;  }
        public ListProductInPut(int page, int perPage, string search, string sort, SearchOrder dir, bool onlyPromotion, Guid categoryId) : base(page, perPage, search, sort, dir)
        {
            OnlyPromotion = onlyPromotion;
            CategoryId = categoryId;
        }

        public ListProductInPut() : base(1, 15, "", "", SearchOrder.Asc)
        {
            OnlyPromotion = false;
            CategoryId = Guid.Empty;
        }
    }
}
