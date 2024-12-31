using Mshop.Application.UseCases.Product.UpdateProduct;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Product.UpdateProduct;
using BusinessEntity = Mshop.Domain.Entity;

namespace Mshop.IntegrationTests.Application.UserCases.Product.UpdateProduct
{
    public class UpdateProdutTestFixture : IntegracaoBaseFixture
    {
        protected UpdateProductInPut RequestFake(Guid Id, Guid categoryId)
        {
            var category = FakerCategory();
            var productFaker = FakerProduct(category);
            var product = (new UpdateProductInPut
            {
                Description = productFaker.Description,
                Name = productFaker.Name,
                Price = Convert.ToDecimal(productFaker.Price),
                Thumb = ImageFake64(),
                CategoryId = categoryId,
                IsActive = true,
                Id = Id    
            });
            return product;
        }

    }
}
