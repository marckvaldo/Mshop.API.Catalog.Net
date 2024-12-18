using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductsOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductsOutPut(int page, int perPage, int total, IReadOnlyList<ProductModelOutPut> itens) 
            : base(page, perPage, total, itens)
        {

        }
    }
}
