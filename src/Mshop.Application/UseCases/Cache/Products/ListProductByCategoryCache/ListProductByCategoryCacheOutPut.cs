﻿using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache
{
    public class ListProductByCategoryCacheOutPut : PaginatedListOutPut<ProductModelOutPut>, IModelOutPut
    {
        public ListProductByCategoryCacheOutPut(int currentPage, int perPage, int total, IReadOnlyList<ProductModelOutPut> data) :
            base(currentPage, perPage, total, data)
        {

        }
    }
}
