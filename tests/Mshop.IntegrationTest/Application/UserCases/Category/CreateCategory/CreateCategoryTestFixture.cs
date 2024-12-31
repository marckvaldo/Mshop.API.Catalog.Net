using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Category.CreateCategory
{
    public class CreateCategoryTestFixture : IntegracaoBaseFixture
    {

        public CreateCategoryInPut FakerCreateCategoryInput()
        {
            var category = FakerCategory();
            return new CreateCategoryInPut
            {
                Name = category.Name,
                IsActive = category.IsActive
            };
        }

    }
}
