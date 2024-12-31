using Mshop.Application.UseCases.Category.UpdateCategory;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Category.UpdateCategory
{
    public class UpdateCategoryTestFixture : IntegracaoBaseFixture
    {
        public UpdateCategoryInPut FakerUpdateCategoryInput()
        {
            var category = FakerCategory();

            return new UpdateCategoryInPut
            {
                Name = category.Name,
                IsActive = category.IsActive,
            };
        }
    }
}
