﻿using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache
{
    public class ListProductPromotionCacheOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductPromotionCacheOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductModelOutPut> itens) :
            base(currentPage, perPage, total, itens)
        {

        }
    }
}
