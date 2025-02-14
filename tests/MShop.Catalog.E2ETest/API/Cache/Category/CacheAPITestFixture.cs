using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Catalog.E2ETests.Base;

namespace Mshop.Catalog.E2ETests.API.Cache.Category
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
