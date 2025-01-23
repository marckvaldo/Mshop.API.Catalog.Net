using Microsoft.AspNetCore.Builder;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Core.Test.Common;
using MShop.Catalog.E2ETests.Base;

namespace MShop.Catalog.E2ETests.API.Cache.Category
{
    public class CacheAPITestFixture : BaseWebApplication
    {
        public CacheAPITestFixture() : base() { }
        public CreateCategoryInPut RequestCreate()
        {
            return new CreateCategoryInPut
            {
                Name = FakerCategory().Name
            };
        }
    }
}
