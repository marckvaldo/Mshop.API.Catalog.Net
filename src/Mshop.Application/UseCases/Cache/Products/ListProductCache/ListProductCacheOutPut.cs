using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductCache
{
    public class ListProductCacheOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductCacheOutPut(int page, int perPage, int total, IReadOnlyList<ProductModelOutPut> itens) :
            base(page, perPage, total, itens)
        {

        }
    }
}
