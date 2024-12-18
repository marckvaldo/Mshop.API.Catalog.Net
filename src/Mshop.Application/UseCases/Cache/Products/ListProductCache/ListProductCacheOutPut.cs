using Microsoft.EntityFrameworkCore.Metadata;
using Mshop.Core.Paginated;
using Mshop.Core.DomainObject;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Application.Common;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCacheOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductCacheOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductModelOutPut> itens) :
            base(currentPage, perPage, total, itens)
        {

        }
    }
}
