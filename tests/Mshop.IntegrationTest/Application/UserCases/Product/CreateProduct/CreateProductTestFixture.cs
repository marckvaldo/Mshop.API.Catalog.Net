using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Product.CreateProduct
{
    public abstract class CreateProductTestFixture : IntegracaoBaseFixture
    {
        protected CreateProductInPut FakerCrateProductInPut(Guid categoryId)
        {
            return new CreateProductInPut
            {
                Name = faker.Commerce.ProductName(),
                Description = faker.Commerce.ProductDescription(),
                Price = Convert.ToDecimal(faker.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = categoryId,
                Stock = faker.Random.UInt(),
                IsActive = true
            };
        }

    }
}
