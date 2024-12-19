using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.UpdateCategory;
using Mshop.Core.Test.UseCase;

namespace Mshop.UnitTests.Application.UseCases.Category.UpdateCategory
{
    public class UpdateCategoryTestFituxre : UseCaseBaseFixture
    {
        public UpdateCategoryInPut FakerRequest()
        {
            return new UpdateCategoryInPut { Name = faker.Commerce.Categories(1)[0], IsActive = true };
        }

        public UpdateCategoryInPut FakerRequest(string name, bool isActive)
        {
            return new UpdateCategoryInPut { Name = name, IsActive = isActive };
        }
    }
}
