using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductsOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductsOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductModelOutPut> data) 
            : base(currentPage, perPage, total, data)
        {

        }
    }
}
