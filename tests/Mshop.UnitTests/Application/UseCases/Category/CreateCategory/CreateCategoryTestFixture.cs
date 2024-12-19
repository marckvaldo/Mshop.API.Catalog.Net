using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Core.Test.UseCase;

namespace Mshop.UnitTests.Application.UseCases.Category.CreateCategory
{
    public class CreateCategoryTestFixture : CategoryBaseFixtureTest
    {
        public CreateCategoryInPut FakerRequest()
        {
            return new CreateCategoryInPut { Name = faker.Commerce.Categories(1)[0], IsActive = true };
        }

        public CreateCategoryInPut FakerRequest(string name, bool isActive)
        {
            return new CreateCategoryInPut { Name = name, IsActive = isActive };
        }
    }
}
