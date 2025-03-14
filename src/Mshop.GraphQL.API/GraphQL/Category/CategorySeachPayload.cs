﻿using Mshop.API.GraphQL.GraphQL.Common;
using Mshop.Application.Common;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.ListCategories;

namespace Mshop.API.GraphQL.GraphQL.Category
{
    public class CategorySeachPayload : PaginatedListOutPut<CategoryPayload>
    {
        public CategorySeachPayload(int currentPage, int perPage, int total, IReadOnlyList<CategoryPayload> data) : 
            base(currentPage, perPage, total, data)
        {

        }
    }
}
