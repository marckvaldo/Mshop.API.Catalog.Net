using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCacheOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductCacheOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductModelOutPut> data) :
            base(currentPage, perPage, total, data)
        {

        }
    }
}
