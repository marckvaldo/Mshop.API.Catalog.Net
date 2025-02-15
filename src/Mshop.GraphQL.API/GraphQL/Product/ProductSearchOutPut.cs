using Mshop.API.GraphQL.GraphQL.Common;
using Mshop.Domain.Entity;
using Polly.Caching;

namespace Mshop.API.GraphQL.GraphQL.Product
{
    public class ProductSearchOutPut : SearchOutPut<ProductPayload>
    {
        public ProductSearchOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductPayload> products) 
            : base(currentPage, perPage, total, products)
        {

        }
    }
}
